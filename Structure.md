# Structure

- Intro of me
  - Intro of Jedlix
- Starting position
- [NOTE] Continuous running demo
- Start with tee of aggregated log file?
  - [SHOW] Chaos!
  - Can you tell what's going on?
- Use Seq to receive log files
  - [SHOW] Aggregated logs in one place
  - Can you tell what's going on?
  - [SHOW] How to Node

------ MAYBE NEED TO INTRODUCE STRUCTURED LOGGING HERE? ------

- Add common properties
  - [SHOW] how we can identify applications
    - Netcore
    - Node
  - Can you tell what's going on?

------ DECIDE IF THIS IS NEEDED ------
- Add structured properties
  - [NOTE] Only add one type of property in one app
  - [SHOW] Ability to find some things going on
  - Can you tell what's going on?
  - [NOTE] Talk about consistency? (NOT SURE HERE OR LATER)
------ SNIP ------

- Tracing
  - What would be good?
  - What options are there? (AWS X-Ray, Jaeger, OpenZipkin, OpenTracing, Google Dapper)
  - What we did
    - Tracing header with correlation id
    - Create and forward
  - [NOTE] Add diagram
  - [SHOW] AspNetCore middleware
  - [SHOW] Node middleware
  - [SHOW] Correlation propagation
    - [NOTE] Correlation context?
- But where is it coming from?
  - [SHOW] Client headers
  - [SHOW] Dashboard?
- Call the front-enders!
  - [NOTE] Talk about headers in mobile apps
  - [SHOW] How to add correlation in Node
  - [SHOW] Find requests from the Node app
  - [NOTE] Talk about consistency
- To infinity and beyond!
  - Think about what adds value for future you (and the other future versions of your team)
  - Short improvement cycles
    - See what works for you
    - Start small
    - With one app
  - Test
  - Think about common properties
  - Did I mention testing?

- **What is the message?**
  - Tracing doesn't need to be hard?
    - But why do we want to have tracing?
      - Because we want to be able to trouble shoot
  - Logging is easy
  - Structured logging is powerful
    - Because:
      - Findability
      - Alerting possibilities
      - Data analysis