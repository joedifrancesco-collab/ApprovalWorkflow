# Approval Workflow — Backlog

## In Progress
_(nothing currently in progress)_

## Ready for Development

### Email Notifications
Send real emails to the requestor when their request is approved or rejected.
- SMTP settings already exist in `appsettings.json` but `Host` is blank
- `IEmailService` interface and placeholder implementation already exist in the codebase
- The approve/reject endpoints in `RequestsController` are the right place to call it
- The frontend already shows a placeholder dialog after approve/reject confirming an email was "sent"

**Acceptance criteria:**
- Requestor receives an email when their request is approved
- Requestor receives an email when their request is rejected
- Email contains the request ID, requestor name, and new status
- If SMTP is not configured, the app logs a warning and continues (no crash)

---

## Ideas / Future Consideration

