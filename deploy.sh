cd Elephanet && nuget pack Elephanet.nuspec -Version $VERSION -IncludeReferencedProjects -Prop Configuration=Release && nuget push *.nupkg $NUGET_API_KEY -verbosity detailed
