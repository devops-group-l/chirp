## System's Perspective
### The onion architecture
![alt text](img/Onion_architecture.png "User sequence")
We chose to work with the onion architecture. The architecture provides loose coupling with the dependencies only
flowing inwards. The interactions between the layers are facilitated through interfaces and abstractions,
hence promoting loose coupling.
### The layers
- The Domain/Core layer
  - `DTOs` (Data Transfer Objects) to transfer data between layers in the system
  - `Interfaces` of the `repositories` to promote loose coupling
- The Infrastructure Layer
  - `Repositories` for data access with the Entity Framework in the `DbContext`
  - `Models`as the entities used in the `DbContext` on the same layer 
  - Additional services such as `Prometheus` and `UpdateLatestTracker` for monitoring the system
- The Webservice/Presentation layer
  - `Controllers` to handle HTTP request and responses with access to the `repositories`
  - `Pages` that contains UI components to render the data to the user. In our project they're referred to as Partials
- Web layer
  - The web layer is outside of the main project and represents outside dependencies `Frameworks`, `Tests` and the `Database`
### Dependencies
We have utilized a range of tools. For coding and development, we have used: C# .NET and PostgreSQL for base functionality. We have used Tailwind CSS for UX/UI improvements. To ensure test-driven-development we have used Playwright and Moq in our test-suite. 

For version control we have used GitHub.

We have hosted our service at Digital Ocean, and done CI/CD through GitHub actions.

For monitoring and logging we have used Grafana and Prometheus, which allowed us to `SKRIV MERE HER` ...

### Interactions of subsystems
  ![alt text](img/User_Sequence.png "User sequence")
- Similarly, another illustrative sequence diagram that shows how requests from the simulator traverse your system.
### State of systems
Describe the current state of your systems, for example using results of static analysis and quality assessments.

## Process' perspective
This perspective should clarify how code or other artifacts come from idea into the running system and everything that happens on the way.
### CI/CD Chain
#### CI Chain
Our CI Chain can be split into the following 3 workflows, where the latter 2 can be considered our main CI chains:
- Dependabot
- Build and test Chain
- Code coverage Chain

##### Dependabot
Dependabot is used to, on a weekly basis, make us aware of dependencies which have new versions, such that we stay up-to-date with our dependency ecosystem.
It creates a PR for each out-of-date dependency, which we can then choose to accept or decline.
This is a built-in bot provided by Github.

##### Build and test CI Chain
The build and test CI chain is a github action, which is run on every pull request to our `itu-minitwit` branch.
This chain attempts to build our project, along with running the tests written for it.

It has the following steps:

| Step Nr. |                 Step Title                  |                                                                              Step Description |
|:--------:|:-------------------------------------------:|----------------------------------------------------------------------------------------------:|
|    1     |                  Checkout                   | Checks out the latest git commit for the given PR, so that it can be used later in the chain. |
|    2     |                 Set up .NET                 |                                Sets up .NET so that the project can be set up, built and run. |
|    3     |            Restore dependencies             |                             Restores the nuget packages which are the project's dependencies. |
|    4     |                Restore tools                |          Restores the .NET tools the project uses (such as `pwsh` to run install playwright). |
|    5     |              Build the project              |      Builds the project. If it fails, the CI chain fails, and thus the Github PR check fails. |
|    6     | Install playwright (required for E2E tests) |                                      Installs playwright, which is used for end-to-end tests. |
|    7     |                  Run tests                  |              Runs tests. If it fails, the CI chain fails, and thus the Github PR check fails. |

This chain ensures that, when a developer implements a feature, the project can build, and works as intended.

##### Code coverage CI Chain
The code coverage CI chain works as an extension to the build and test CI chain, since it both builds and tests the project, in addition to creating a test coverage report.
It is run, along with the build and test CI chain, on every created pull request to our `itu-minitwit` branch.

It has the following steps:

| Step Nr. |                     Step Title                     |                                                                                                                                                                                                       Step Description |
|:--------:|:--------------------------------------------------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|    1     |                      Checkout                      |                                                                                                                          Checks out the latest git commit for the given PR, so that it can be used later in the chain. |
|    2     |                    Set up .NET                     |                                                                                                                                                         Sets up .NET so that the project can be set up, built and run. |
|    3     |                Restore dependencies                |                                                                                                                                                      Restores the nuget packages which are the project's dependencies. |
|    4     |                   Restore tools                    |                                                                                                                                   Restores the .NET tools the project uses (such as `pwsh` to run install playwright). |
|    5     |                 Build the project                  |                                                                                                                               Builds the project. If it fails, the CI chain fails, and thus the Github PR check fails. |
|    6     |    Install playwright (required for E2E tests)     |                                                                                                                                                               Installs playwright, which is used for end-to-end tests. |
|    7     |              Run tests with coverage               | Runs tests. If it fails, the CI chain fails, and thus the Github PR check fails. The tests are run using the `dotcover` tool, which generates a code coverage report, using the `dotcoverConfig.xml` as configuration. |
|    8     |                  Report Generator                  |                                                    Takes the dotcover-generated `analysis.xml` report, and transforms it to a `Cobertura` format that can be used to create a PR message showcasing the code coverage. |
|    9     |         Build Code Coverage Summary Report         |                                                                                   Takes the `Cobertura` report, and converts it to a markdown format, which can be used to showcase the code coverage as a PR comment. |
|    10    |              Add Coverage PR Comment               |                                                                                                                This step takes the generated markdown-format code coverage report, and adds it as a sticky PR comment. |
|    11    |                Write to Job Summary                |                                                                                                    This step takes the generated markdown-format code coverage report, and adds it as a summary to the given workflow. |
|    12    | Test Check Failure of Code Coverage Summary Report |                                                      Takes the code coverage report, and given a minimum threshold of 60%, fails the check if the code coverage % is below 60%. If below 80%, only a warning is given. |

This chain ensures that enough of the code is tested at all times.
Furthermore, it is also used as a reminder for the developers to write tests to their newly implemented features.

##### CI Chain reflections
Looking back at it now, we should have been better at utilising and respecting our CI chains.
This is mainly due to there being other more pressing tasks for this project, which ended up taking most of the time we allocated to the project.

Although setting up Dependabot, we weren't good at continuously updating the recommended dependencies, which we should have done as a weekly ritual.

Furthermore, early on in the project, we had commented out our 2 primary CI chains during a major refactoring of the project, and had forgotten to un-comment it.
Looking back at it, we should have never uncommented them, but rather just adjust to them, as it could have led to some serious consequences.
Moreover, we could probably have done without the build and test chain, and just simply used the code coverage chain alone instead, as it will also fail if the project can't build or the tests fail.

#### CD Chain
Our CD Chain can be split into the following 2 workflows:
- Publish App Chain
- Continuous Deployment Chain

##### Publish App CD Chain
Our publish app CD chain is run on a schedule, which is every Thursday. It is responsible for creating a minor release at once a week.

It can be split into the `prepare release` and `release app` jobs, where the latter job runs as a matrix for the OS' `linux-x64`, `win-x64` and `osx-x64`.
It has the following steps:

| Step Nr. |                     Step Title                     |                                                                                                                                                                   Step Description |
|:--------:|:--------------------------------------------------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|    1     |                 Checkout (prepare)                 | Checks out the given commit history for the `itu-minitwit` branch. It does so with a `fetch-depth: 0` to ensure we get the history of all tags, which are used later in the chain. |
|    2     |        Determine new version tag (prepare)         |                                                                                                Gets the latest tag, and adds 1 to the minor version, to create the new future tag. |
|    3     |                 Checkout (release)                 |                                                                                      Checks out the latest git commit for the given PR, so that it can be used later in the chain. |
|    4     |               Set up .NET (release)                |                                                                                                                     Sets up .NET so that the project can be set up, built and run. |
|    5     |            Build and publish (release)             |                                                                                                 Builds and publishes the project to a zip file given the current OS in the matrix. |
|    6     |         Create and push new tag (release)          |                                                                                                                    Creates the new tag given the `Determine new version tag` step. |
|    7     |               Publish app (release)                |                                                                                 Generates a release if not yet existent given the new tag, and adds the generated zip files to it. |

This ensures that we continuously deliver releases.

[//]: # (TODO: Something about why we need to continuously deliver releases?)

##### Continuous Deployment CD Chain
In Digital Ocean, we have already provisioned 2 VMs, along with 2 reserved IPs pointing to them, following the green-blue deployment model.
Our CD chain is responsible for updating our VMs in Digital Ocean, and switch the reserved IPs in order to minimise downtime during deployments.
This CD chain is run on every push to the `itu-minitwit` branch, and can be triggered manually.

It has the following steps:

| Step Nr. |            Step Title             |                                                                                                                                         Step Description |
|:--------:|:---------------------------------:|---------------------------------------------------------------------------------------------------------------------------------------------------------:|
|    1     |             Checkout              |                                                            Checks out the latest git commit for the given PR, so that it can be used later in the chain. |
|    2     |        Login to Docker Hub        |                                    Given secrets in our github repository, logs in to Docker Hub, so that we later can push our image to our repository. |
|    3     |       Set up Docker Buildx        |                                                                                                                                   Sets up Docker Buildx. |
|    4     |   Build and push minitwitimage    |                                         Builds an image from our Dockerfile, and pushes it to our repository on Docker Hub, along with the tag `latest`. |
|    5     |           Configure SSH           |      Adds the SSH key, which is stored in our github repository's secrets, to our working directory. This is so we can SSH to our VM later in the chain. |
|    6     |         Deploy to server          | We SSH into our green droplet/VM, and run the `deploy.sh` script, which pulls our newly created image, and re-runs the container through docker-compose. |
|    7     |        Get blue droplet id        |                                                                       We fetch the blue droplet id, so we later can use it to reassign our reserved IPs. |
|    8     |       Get green droplet id        |                                                                      We fetch the green droplet id, so we later can use it to reassign our reserved IPs. |
|    9     |        Un-assign green IP         |                                               We un-assign the reserved IP pointing to our green droplet, so we later can re-assign to our blue droplet. |
|    10    |            Wait 30secs            |                                                                           We wait 30 seconds whilst Digital Ocean finishes un-assigning the reserved IP. |
|    11    | Reassign blue IP to green droplet |                   We reassign the blue reserved IP to the green droplet, which is running with our latest Docker image and becomes our new blue droplet. |
|    12    |            Wait 30secs            |                                                                            We wait 30 seconds whilst Digital Ocean finishes reassigning the reserved IP. |
|    13    | Reassign green IP to blue droplet |                                                           We reassign the green reserved IP to the old blue droplet, which is not our new green droplet. |

In short, when we start a deployment, we:
- Build and upload the new docker image to Docker Hub
- SSH into our green droplet
- Update the Docker image and restart using docker-compose
- Switch reserved IPs

This ensures that we continuously deliver the latest version of our application, whilst minimising our downtime during deployment thanks to our green-blue deployment model.
In this scenario, we decided to use Digital Ocean's API to make the switch between green and blue droplets, and have used the reserved IPs as our router.
Furthermore, with this CD pipeline, we can also easily can go back to the "non-live" droplet by switching the reserved IPs, in case the live droplet is somehow erroneous.

The main potential weakness we see with this CD chain, is if case 2 deployments are started in quick succession.
In that case, we don't know how the deployments will interact.

For a more robust CD chain, we should probably ensure that only one deployment can run at once.
However, due to time constraints, we weren't able to implement this.

### Monitoring
How do you monitor your systems and what precisely do you monitor?
### Logging
What do you log in your systems and how do you aggregate logs?
### Security
Brief results of the security assessment and brief description of how did you harden the security of your system based on the analysis
### Strategy for scaling and upgrades
Applied strategy for scaling and upgrades
### Reflection on AI assistance
During the project we've used ChatGPT to search for information and code snippets for guidance.
It is a mutual opinion in the group that copy-pasting our code to any chat bot or other AI tools should be avoided
as we're unaware of where the data is stored and exactly who has access. Therefor we view AI tools such as ChatGPT 
as an addition to Google where we can search for information and get answers to our questions.
Additionally, manual development has the best learning outcome.

## Lessons Learned Perspective
Describe the biggest issues, how you solved them, and which are major lessons learned with regards to:

### Continuous refactoring
At the beginning of the simulation, we implemented functionality for receiving HTTP requests and storing data in separate tables unrelated to 'real' data. This data served the relevant HTTP simulation endpoints, but was purposely not visible on our applications feed. 
Later on we then had to display this simulation data, which initially prompted us to change how we processed incoming requests. However this would require a migration, which we feared could cause a wipe of our database. 
Therefore we had to figure out a way to transform incoming data into visible data, without changing data-models (and causing a migration). Ultimately we made it possible to transform this data in feed-viable data, without changing models. 
Through this we learned the value of `SKRIV MERE HER`

- evolution and refactoring
- operation, and
- maintenance

of your ITU-MiniTwit systems. Link back to respective commit messages, issues, tickets, etc. to illustrate these.

Also reflect and describe what was the "DevOps" style of your work. For example, what did you do differently to previous development projects and how did it work?

