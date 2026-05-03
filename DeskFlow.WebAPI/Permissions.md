### 1- User 
Permissions| Admin | Manager | Technician | User
---|---|---|---|---
Create user | Yes | No | No | No
Delete user | Yes | No | No | No
Edit user | Yes | No | No | No
Get user details | Yes | In their Department | self | self
Get all users | Yes | In their Department | No | No

### 2- Ticket
Permissions| Admin | Manager | Technician | User
---|---|---|---|---
Create ticket | Yes | Yes | yes | yes
Delete ticket | Yes | No | No | No
update ticket description | No | No | No | yes
View Tickets | All| Assigned to theit Department | Assigned to them | Reported by them
update ticket status | No | Yes | Yes | No
update ticket priority | No | Yes | No | No
Assign ticket to technician | No | Yes | Can Assign themself | No

### 3- Department
Permissions| Admin | Manager | Technician | User
---|---|---|---|---
Create department | Yes | No | No | No
Delete department | Yes | No | No | No
get all departments | Yes | No | No | No

### 4- SLA
Permissions| Admin | Manager | Technician | User
---|---|---|---|---
Create SLA | Yes | No | No | No
Delete SLA | Yes | No | No | No
Update SLA | Yes | No | No | No
Get all SLAs | Yes | No | No | No


