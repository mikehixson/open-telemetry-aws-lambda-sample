param (
    [switch]$AutoApprove = $false
)

dotnet lambda package -farch arm64 -pl "..\source\OTelAwsLambdaSample.Lambda"

if ($AutoApprove) {
    terraform -chdir="..\terraform" apply -auto-approve    
}
else {
    terraform -chdir="..\terraform" apply
}