# Understanding SQL Injection

SQL Injection is a security vulnerability that allows an attacker to interfere with the queries an application makes to its database. It is one of the most common and dangerous web vulnerabilities.

---

## What is SQL Injection?

SQL Injection occurs when untrusted user input is included directly in a SQL query without proper validation or escaping. Attackers can manipulate the input to execute arbitrary SQL code, potentially gaining unauthorized access to data or even modifying or deleting data.

---

## Example of SQL Injection

Suppose you have code like this (bad practice!):

```csharp
string query = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{password}'";
```
If an attacker enters:
- `username`: `admin' --`
- `password`: `anything`

The query becomes:
```sql
SELECT * FROM Users WHERE Username = 'admin' --' AND Password = 'anything'
```
The `--` starts a comment, so the password check is ignored. The attacker logs in as `admin` without knowing the password!

---

## How to Prevent SQL Injection

1. **Use Parameterized Queries (Recommended):**
   ```csharp
   var cmd = new SqlCommand("SELECT * FROM Users WHERE Username = @username AND Password = @password", connection);
   cmd.Parameters.AddWithValue("@username", username);
   cmd.Parameters.AddWithValue("@password", password);
   ```
2. **Use ORM Frameworks:**
   - Entity Framework Core automatically parameterizes queries, making injection much harder.
3. **Validate and Sanitize Input:**
   - Never trust user input. Validate and sanitize all inputs.
4. **Least Privilege Principle:**
   - The database user should have only the permissions it needs.
5. **Regularly Update Libraries:**
   - Keep your frameworks and libraries up to date to avoid known vulnerabilities.

---

## Key Interview Points
- SQL Injection is a critical security risk.
- Always use parameterized queries or ORM frameworks.
- Never concatenate user input directly into SQL statements.
- Validate and sanitize all user input.

---

**Learn more:**
- [OWASP SQL Injection](https://owasp.org/www-community/attacks/SQL_Injection)
- [Microsoft Docs: SQL Injection](https://learn.microsoft.com/en-us/sql/relational-databases/security/sql-injection)
