# OhpenBankingMigration
Technical assesment for Ohpen

# General considerations
The solution for this assesment consists of two main projects: 
1- BankingMigration.Api: the web api that receives migration requests
2- BankingMigration.Func: An Azure Function App triggered by Azure Service Bus Queue, that actually perfoms the migration process.
As the migration of a single entity by the external api takes 500 miliseconds, we decided to implement an asynchronous architecture.
All the rest is supposed to be self explenatory.

# Running the proyect
## Prerequisites
1- Docker and Docker Compose installed (or Docker Desktop installed).

## Step by step process
1- Open CMD at the .sln file level and run "docker compose up -d" build and run an SQL Server Docker Container  
2- Run or Debug the Migration.Api proyect.  
3- If you have a Postman account, import the collection stored in the file Ohpen_Assesment.postman_collection.json  
4- Execute the requests "Request Bulk Migration" or "Request Batch Migration"  
5- The values returned are the ids of the requests.  
6- Run or Debug the BankingMigration.Func project  
7- Execute the Postman request "Run Migration AZ Function" with one of the ids returned in step 4  
8- Run or Debug Migration.Api proyect and execute the Postman request "Check Migration Status" with the id passed to the AZ Function request  
# Proyect CI/CD
The project contains the file .github/workflow/banking-migration-docker-image.yml containing the continous delivery pipeline. The proyect is delivered as Docker container images ready to be deployed in any cloud or on premise environment. Two images are provided: one for the BankingMigration.Api proyect and another for the BankingMigration.Func project.  
The images are distributed via Docker Hub container registry.  
The images repositories are:  
1- https://hub.docker.com/repository/docker/rolandomesagdp/banking-migration-api/general  
2- https://hub.docker.com/repository/docker/rolandomesagdp/banking-migration-az-func/general  
## CI/CD Workflow trigger
The Github workflow is triggered on every push or pull request to the main branch.
