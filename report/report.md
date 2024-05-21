## System's Perspective
### Design and architecture
![alt text](img/Onion_architecture.png "User sequence")
Design and architecture of your ITU-MiniTwit systems
### Dependencies
All dependencies of your ITU-MiniTwit systems on all levels of abstraction and development stages. That is, list and briefly describe all technologies and tools you applied and depend on.
### Interactions of subsystems
- For example, via an illustrative UML Sequence diagram that shows the flow of information through your system from user request in the browser, over all subsystems, hitting the database, and a response that is returned to the user.
  ![alt text](img/User_Sequence.png "User sequence")
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
In case you have used AI-assistants during your project briefly explain which system(s) you used during the project and reflect how it supported/hindered your process.

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

