// Code generated by protoc-gen-go-grpc. DO NOT EDIT.
// versions:
// - protoc-gen-go-grpc v1.2.0
// - protoc             v3.19.4
// source: src/checker/cmd/server/grpc/.proto

package grpc

import (
	context "context"
	grpc "google.golang.org/grpc"
	codes "google.golang.org/grpc/codes"
	status "google.golang.org/grpc/status"
)

// This is a compile-time assertion to ensure that this generated file
// is compatible with the grpc package it is being compiled against.
// Requires gRPC-Go v1.32.0 or later.
const _ = grpc.SupportPackageIsVersion7

// StagesServiceClient is the client API for StagesService service.
//
// For semantics around ctx use and closing/ending streaming RPCs, please refer to https://pkg.go.dev/google.golang.org/grpc/?tab=doc#ClientConn.NewStream.
type StagesServiceClient interface {
	SaveStages(ctx context.Context, in *SaveStagesRequest, opts ...grpc.CallOption) (*Empty, error)
}

type stagesServiceClient struct {
	cc grpc.ClientConnInterface
}

func NewStagesServiceClient(cc grpc.ClientConnInterface) StagesServiceClient {
	return &stagesServiceClient{cc}
}

func (c *stagesServiceClient) SaveStages(ctx context.Context, in *SaveStagesRequest, opts ...grpc.CallOption) (*Empty, error) {
	out := new(Empty)
	err := c.cc.Invoke(ctx, "/grpc.StagesService/SaveStages", in, out, opts...)
	if err != nil {
		return nil, err
	}
	return out, nil
}

// StagesServiceServer is the server API for StagesService service.
// All implementations must embed UnimplementedStagesServiceServer
// for forward compatibility
type StagesServiceServer interface {
	SaveStages(context.Context, *SaveStagesRequest) (*Empty, error)
	mustEmbedUnimplementedStagesServiceServer()
}

// UnimplementedStagesServiceServer must be embedded to have forward compatible implementations.
type UnimplementedStagesServiceServer struct {
}

func (UnimplementedStagesServiceServer) SaveStages(context.Context, *SaveStagesRequest) (*Empty, error) {
	return nil, status.Errorf(codes.Unimplemented, "method SaveStages not implemented")
}
func (UnimplementedStagesServiceServer) mustEmbedUnimplementedStagesServiceServer() {}

// UnsafeStagesServiceServer may be embedded to opt out of forward compatibility for this service.
// Use of this interface is not recommended, as added methods to StagesServiceServer will
// result in compilation errors.
type UnsafeStagesServiceServer interface {
	mustEmbedUnimplementedStagesServiceServer()
}

func RegisterStagesServiceServer(s grpc.ServiceRegistrar, srv StagesServiceServer) {
	s.RegisterService(&StagesService_ServiceDesc, srv)
}

func _StagesService_SaveStages_Handler(srv interface{}, ctx context.Context, dec func(interface{}) error, interceptor grpc.UnaryServerInterceptor) (interface{}, error) {
	in := new(SaveStagesRequest)
	if err := dec(in); err != nil {
		return nil, err
	}
	if interceptor == nil {
		return srv.(StagesServiceServer).SaveStages(ctx, in)
	}
	info := &grpc.UnaryServerInfo{
		Server:     srv,
		FullMethod: "/grpc.StagesService/SaveStages",
	}
	handler := func(ctx context.Context, req interface{}) (interface{}, error) {
		return srv.(StagesServiceServer).SaveStages(ctx, req.(*SaveStagesRequest))
	}
	return interceptor(ctx, in, info, handler)
}

// StagesService_ServiceDesc is the grpc.ServiceDesc for StagesService service.
// It's only intended for direct use with grpc.RegisterService,
// and not to be introspected or modified (even as a copy)
var StagesService_ServiceDesc = grpc.ServiceDesc{
	ServiceName: "grpc.StagesService",
	HandlerType: (*StagesServiceServer)(nil),
	Methods: []grpc.MethodDesc{
		{
			MethodName: "SaveStages",
			Handler:    _StagesService_SaveStages_Handler,
		},
	},
	Streams:  []grpc.StreamDesc{},
	Metadata: "src/checker/cmd/server/grpc/.proto",
}
