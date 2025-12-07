# Tourist Agency Minikube Cleanup Script
# Run this script from the root directory (agency-api)

Write-Host "🧹 Cleaning up Tourist Agency from Minikube..." -ForegroundColor Yellow

# Delete all resources
Write-Host "🗑️ Deleting Kubernetes resources..." -ForegroundColor Yellow
kubectl delete -k k8s/

# Wait for resources to be deleted
Write-Host "⏳ Waiting for resources to be deleted..." -ForegroundColor Yellow
kubectl wait --for=delete namespace/tourist-agency --timeout=60s

# Check if namespace is deleted
$namespaceExists = kubectl get namespace tourist-agency 2>$null
if ($namespaceExists) {
    Write-Host "⚠️ Namespace still exists, forcing deletion..." -ForegroundColor Yellow
    kubectl delete namespace tourist-agency --force --grace-period=0
}

# Remove Docker images (optional)
$removeImages = Read-Host "Do you want to remove Docker images? (y/N)"
if ($removeImages -eq "y" -or $removeImages -eq "Y") {
    Write-Host "🐳 Removing Docker images..." -ForegroundColor Yellow
    & minikube docker-env | Invoke-Expression
    docker rmi tourist-agency-auth:latest 2>$null
    docker rmi tourist-agency-api:latest 2>$null
    docker rmi tourist-agency-ui:latest 2>$null
    Write-Host "✅ Docker images removed" -ForegroundColor Green
}

Write-Host "✅ Cleanup complete!" -ForegroundColor Green