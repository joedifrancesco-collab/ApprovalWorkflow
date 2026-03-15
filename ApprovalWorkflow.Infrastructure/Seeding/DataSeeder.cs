using ApprovalWorkflow.Application.Interfaces;
using ApprovalWorkflow.Domain;
using BC = BCrypt.Net.BCrypt;

namespace ApprovalWorkflow.Infrastructure.Seeding;

public class DataSeeder
{
    private readonly IUserRepository         _userRepository;
    private readonly IUserRoleRepository     _userRoleRepository;
    private readonly IUserXRoleRepository    _userXRoleRepository;
    private readonly ITierRepository         _tierRepository;
    private readonly IStatusRepository       _statusRepository;
    private readonly ILocationRepository     _locationRepository;

    public DataSeeder(
        IUserRepository      userRepository,
        IUserRoleRepository  userRoleRepository,
        IUserXRoleRepository userXRoleRepository,
        ITierRepository      tierRepository,
        IStatusRepository    statusRepository,
        ILocationRepository  locationRepository)
    {
        _userRepository      = userRepository;
        _userRoleRepository  = userRoleRepository;
        _userXRoleRepository = userXRoleRepository;
        _tierRepository      = tierRepository;
        _statusRepository    = statusRepository;
        _locationRepository  = locationRepository;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedUserEmailsAsync();
        await SeedUserRoleAssignmentsAsync();
        await SeedTiersAsync();
        await SeedStatusesAsync();
        await SeedLocationsAsync();
    }

    // Upsert roles by SortOrder � renames Submitter?Requestor and adds Admin
    private async Task SeedRolesAsync()
    {
        var desired = new List<UserRole>
        {
            new() { RoleName = "Reader",      RoleDescription = "Read only permission",       SortOrder = 10 },
            new() { RoleName = "Requestor",   RoleDescription = "Read and submit changes",    SortOrder = 20 },
            new() { RoleName = "Approver",    RoleDescription = "Read and approve requests",  SortOrder = 30 },
            new() { RoleName = "Coordinator", RoleDescription = "Read and Batch coordinator", SortOrder = 40 },
            new() { RoleName = "Admin",       RoleDescription = "Full control",               SortOrder = 50 },
        };

        var existing = (await _userRoleRepository.GetAllAsync()).ToDictionary(r => r.SortOrder);

        foreach (var role in desired)
        {
            if (existing.TryGetValue(role.SortOrder, out var ex))
            {
                ex.RoleName        = role.RoleName;
                ex.RoleDescription = role.RoleDescription;
                await _userRoleRepository.UpdateAsync(ex);
            }
            else
            {
                await _userRoleRepository.CreateAsync(role);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userRepository.AnyAsync())
            return;

        var users = new List<(string First, string Last, string Number, string Password, string Email)>
        {
            ("Alice",   "Johnson",  "10000001", "pass1", "alice.johnson@example.com"),
            ("Bob",     "Smith",    "10000002", "pass2", "bob.smith@example.com"),
            ("Carol",   "White",    "10000003", "pass3", "carol.white@example.com"),
            ("David",   "Brown",    "10000004", "pass4", "david.brown@example.com"),
            ("Emma",    "Davis",    "10000005", "pass5", "emma.davis@example.com"),
            ("Frank",   "Wilson",   "10000006", "pass6", "frank.wilson@example.com"),
            ("Grace",   "Lee",      "10000007", "pass7", "grace.lee@example.com"),
            ("Henry",   "Taylor",   "10000008", "pass8", "henry.taylor@example.com"),
            ("Iris",    "Martinez", "10000009", "pass9", "iris.martinez@example.com"),
            ("Joe",     "King",     "12345678", "pass",  "joe.king@example.com"),
        };

        foreach (var (first, last, number, password, email) in users)
        {
            await _userRepository.CreateAsync(new User
            {
                FirstName    = first,
                LastName     = last,
                UserNumber   = number,
                Email        = email,
                PasswordHash = BC.HashPassword(password)
            });
        }
    }

    private async Task SeedUserRoleAssignmentsAsync()
    {
        var roles = (await _userRoleRepository.GetAllAsync()).ToDictionary(r => r.RoleName, r => r.RoleID);
        if (!roles.Any()) return;

        // Joe King = Admin; others spread across roles
        var assignments = new List<(string UserNumber, string RoleName)>
        {
            ("10000001", "Reader"),
            ("10000002", "Requestor"),
            ("10000003", "Approver"),
            ("10000004", "Coordinator"),
            ("10000005", "Reader"),
            ("10000006", "Requestor"),
            ("10000007", "Approver"),
            ("10000008", "Coordinator"),
            ("10000009", "Approver"),
            ("12345678", "Admin"),
        };

        foreach (var (userNumber, roleName) in assignments)
        {
            var user = await _userRepository.GetByUserNumberAsync(userNumber);
            if (user is null) continue;

            var existing = await _userXRoleRepository.GetByUserIdAsync(user.Id);
            if (existing is not null) continue;

            if (!roles.TryGetValue(roleName, out var roleId)) continue;

            await _userXRoleRepository.CreateAsync(new UserXRole { UserID = user.Id, RoleID = roleId });
        }
    }

    private async Task SeedTiersAsync()
    {
        if (await _tierRepository.AnyAsync()) return;

        foreach (var name in new[] { "Red", "Green", "Blue", "Black" })
            await _tierRepository.CreateAsync(new Tier { TierName = name });
    }

    private async Task SeedStatusesAsync()
    {
        var existing = (await _statusRepository.GetAllAsync()).Select(s => s.StatusName).ToHashSet();
        foreach (var name in new[] { "Unknown", "Awaiting Approval", "Approved", "Rejected", "Printed", "Active", "Inactive" })
        {
            if (!existing.Contains(name))
                await _statusRepository.CreateAsync(new RequestStatus { StatusName = name });
        }
    }

    private async Task SeedLocationsAsync()
    {
        if (await _locationRepository.AnyAsync()) return;

        foreach (var name in new[] { "Orlando", "Tampa", "Texas", "California" })
            await _locationRepository.CreateAsync(new Location { LocationName = name });
    }

    // Backfill emails for seeded users that were created before the Email column was added
    private async Task SeedUserEmailsAsync()
    {
        var emailMap = new Dictionary<string, string>
        {
            ["10000001"] = "alice.johnson@example.com",
            ["10000002"] = "bob.smith@example.com",
            ["10000003"] = "carol.white@example.com",
            ["10000004"] = "david.brown@example.com",
            ["10000005"] = "emma.davis@example.com",
            ["10000006"] = "frank.wilson@example.com",
            ["10000007"] = "grace.lee@example.com",
            ["10000008"] = "henry.taylor@example.com",
            ["10000009"] = "iris.martinez@example.com",
            ["12345678"] = "joe.king@example.com",
        };

        foreach (var (number, email) in emailMap)
        {
            var user = await _userRepository.GetByUserNumberAsync(number);
            if (user is not null && string.IsNullOrEmpty(user.Email))
                await _userRepository.UpdateEmailAsync(user.Id, email);
        }
    }
}
