# Introduction 
Tools for ruuning ASP.NET Core services locally in isolation. 

# Getting Started
`ServicesTestFramework.WebAppTools` contains extension classes that help start and configure service to run locally.
`ServicesTestFramework.DatabaseContainers` contains classes for starting docker container with DB and applying migrations to that DB.

# Known issues
- The project containing the tests should have a direct dependency on `Microsoft.AspNetCore.Mvc.Testing` despite that there is transitive dependency from `ServicesTestFramework.WebAppTools`.