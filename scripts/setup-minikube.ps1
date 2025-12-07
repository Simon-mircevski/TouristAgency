# Tourist Agency Minikube Setup Script
# Run this script from the root directory (agency-api)

Write-Host "🚀 Setting up Tourist Agency on Minikube..." -ForegroundColor Green

# Check if Minikube is running
Write-Host "📋 Checking Minikube status..." -ForegroundColor Yellow
$minikubeStatus = minikube status --format="json" | ConvertFrom-Json
if ($minikubeStatus.Host -ne "Running") {
    Write-Host "❌ Minikube is not running. Starting Minikube..." -ForegroundColor Red
    minikube start --memory=4096 --cpus=2
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Failed to start Minikube" -ForegroundColor Red
        exit 1
    }
}

# Set Docker environment to Minikube
Write-Host "🐳 Configuring Docker environment for Minikube..." -ForegroundColor Yellow
& minikube docker-env | Invoke-Expression

# Build Docker images
Write-Host "🔨 Building Docker images..." -ForegroundColor Yellow

Write-Host "  Building Auth Service..." -ForegroundColor Cyan
docker build -t tourist-agency-auth:latest ./TouristAgencyAuth/TouristAgencyAuth
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to build auth service" -ForegroundColor Red
    exit 1
}

Write-Host "  Building API Gateway..." -ForegroundColor Cyan
docker build -t tourist-agency-api:latest ./TouristAgencyAPI
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to build API gateway" -ForegroundColor Red
    exit 1
}

Write-Host "  Building Frontend..." -ForegroundColor Cyan
docker build -t tourist-agency-ui:latest ./TouristAgencyUI
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to build frontend" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Docker images built successfully!" -ForegroundColor Green

# Deploy to Kubernetes
Write-Host "🚀 Deploying to Kubernetes..." -ForegroundColor Yellow
kubectl apply -k k8s/
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to deploy to Kubernetes" -ForegroundColor Red
    exit 1
}

# Wait for deployments to be ready
Write-Host "⏳ Waiting for deployments to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=available --timeout=300s deployment/mysql -n tourist-agency
kubectl wait --for=condition=available --timeout=300s deployment/auth-service -n tourist-agency
kubectl wait --for=condition=available --timeout=300s deployment/api-gateway -n tourist-agency
kubectl wait --for=condition=available --timeout=300s deployment/frontend -n tourist-agency

# Check deployment status
Write-Host "📊 Checking deployment status..." -ForegroundColor Yellow
kubectl get pods -n tourist-agency
kubectl get services -n tourist-agency

# Get service URLs
Write-Host "🌐 Service URLs:" -ForegroundColor Green
Write-Host "  Frontend: " -NoNewline
minikube service frontend -n tourist-agency --url
Write-Host "  API Gateway: " -NoNewline
minikube service api-gateway -n tourist-agency --url
Write-Host "  Auth Service: " -NoNewline
minikube service auth-service -n tourist-agency --url

Write-Host "✅ Setup complete! You can now access the services using the URLs above." -ForegroundColor Green
Write-Host "💡 Tip: Use 'kubectl logs -f deployment/<service-name> -n tourist-agency' to view logs" -ForegroundColor Cyan