# Migration status

The legacy application has 202 Web Forms code-behind files. The new API is deliberately additive: the original site is untouched and can continue serving existing users while mobile or web clients move to bearer-token APIs.

Implemented resource groups:

- Authentication and legacy password compatibility
- Users and current-user profile
- Schools
- Students read endpoints
- School invoices and invoice detail/create
- School payments and payment create
- Accounts dashboard daily collection
- Classes create/read/update/delete
- Academic years and terms lookups
- Subjects read/create
- Exams read/create
- Student registration with parent record and admission number
- Class-wide fee invoice generation
- Fee payment posting with balance update and receipt number
- Exam mark roster and grade upsert
- Report-card marks, grading-system, and remarks reads
- Configured SMS sending
- Licence listing and school licence details
- Licence generation and activation
- Student edit, soft delete, recovery, and deleted-student listing
- Ranked exam results by class
- Multipart school registration with logo/letterhead storage
- Receipt detail retrieval
- Class-teacher assignment and update
- Bulk receipt data by date, class, or transaction IDs
- Payment reports by detail, category, mode, class, and total

The remaining screens fall into these groups and should be ported into the same controllers/services before declaring feature parity: PDF/download output, detailed result/report-card presentation, and role-specific reports. The API now exposes the underlying JSON data for the bulk receipt and financial report screens; client-specific PDF or thermal-printer rendering remains a presentation concern.

## Password reset table

Create this table before using `POST /api/users/forgot-password`:

```sql
CREATE TABLE PasswordResetTokens (
	PasswordResetTokenId int IDENTITY(1,1) PRIMARY KEY,
	UserId int NOT NULL,
	TokenHash varchar(64) NOT NULL,
	ExpiresAt datetime2 NOT NULL,
	CreatedAt datetime2 NOT NULL,
	CONSTRAINT FK_PasswordResetTokens_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
CREATE UNIQUE INDEX UX_PasswordResetTokens_UserId ON PasswordResetTokens(UserId);
CREATE INDEX IX_PasswordResetTokens_TokenHash ON PasswordResetTokens(TokenHash);
```

The API returns the reset token because no email or SMS delivery provider is configured in this project. A trusted client or delivery service should send that token to the user, then call `POST /api/users/reset-password`.
