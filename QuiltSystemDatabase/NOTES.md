# Database Notes

## Package Manager Console Commands

```
Add Microsoft.EntityFrameworkCore.SqlServer
Add Microsoft.EntityFrameworkCore.Tools
Add Bricelam.EntityFrameworkCore.Pluralizer

Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QuiltCoreModel' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Model

Scaffold-DbContext 'Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QuiltCoreModel' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Database\Model -Context QuiltContext -Force
```