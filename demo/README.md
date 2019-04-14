# Logging and Tracing demo

## Structure

- Car API (2 instances)
- User API (2 instances)
- Mobile API (2 instances)
- Mobile client (3 instances)
- Cars (3 instances)
- Load balancer for each API
- Seq to capture logs

- Continuously have the cars supplying data, randomize intervals
- Mobile apps make requests continuously, randomize intervals

## Links

- [Seq](http://localhost:8080)
- [Load balancer dashboard Car API](http://localhost:1936)
- [Load balancer dashboard User API](http://localhost:1937)
- [Load balancer dashboard Mobile API](http://localhost:1938)