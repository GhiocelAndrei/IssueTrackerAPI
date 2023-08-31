## Installing the Chart

helm install issue-tracker deploy/ --set connectionString.SqlServer="Data Source=host.docker.internal\\SQLEXPRESS; Initial Catalog=IssueTrackerDb; User ID=sa;Password=1qazXSW@; trustServerCertificate=true"

## Accessing the API :

http://localhost:30141

## Example :

http://localhost:30141/api/issues/1
