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
All dependencies of your ITU-MiniTwit systems on all levels of abstraction and development stages. That is, list and briefly describe all technologies and tools you applied and depend on.
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
- Build and test chain
- Code coverage chain

Dependabot is used to, on a weekly basis, make us aware of dependencies which have new versions, such that we stay up-to-date.
We were, however not very good at continuously updating the recommended dependencies, mainly due to other more pressing tasks for this project.

##### Build and test CI chain
Build and test CI chain is a github action, which is run on every pull request to our main branch.
This chain attempts to build our project, along with running the tests written for it.
It has the following steps:
1. Checkout latest git revision
2. Set up .NET
3. Restore dependencies
4. Restore tools
5. Build the project
6. Install playwright (required for E2E tests)
7. Run tests

- Description of stages and tools
- Deploys or releases?

##### Code coverage CI chain
- Description of stages and tools
- Deploys or releases?

#### CD Chain
- Publish App
- Continuous Deployment

##### Publish App CD chain
- Description of stages and tools
- Deploys or releases?
##### Continuous Deployment CD chain
- Description of stages and tools
- Deploys or releases?

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

