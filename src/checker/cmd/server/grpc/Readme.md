In project's root:

```
protoc --go_out=./src/checker/cmd/server/ --go-grpc_out=./src/checker/cmd/server/ ./src/checker/cmd/server/grpc/.proto
```
