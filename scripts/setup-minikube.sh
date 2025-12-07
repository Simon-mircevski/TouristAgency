#!/bin/bash

# Tourist Agency Minikube Setup Script
# Run this script from the root directory (agency-api)

echo "🚀 Setting up Tourist Agency on Minikube..."

# Check if Minikube is running
echo "📋 Checking Minikube status..."
if ! minikube status | grep -q "Running"; then
    echo "❌ Minikube is not running. Starting Minikube..."
    minikube start --memory=4096 --cpus=2
    if [ $? -ne 0 ]; then
        echo "❌ Failed to start Minikube"
        exit 1
    fi
fi

# Set Docker environment to Minikube
echo "🐳 Configuring Docker environment for Minikube..."
eval $(minikube docker-env)

# Build Docker images
echo "🔨 Building Docker images..."

echo "  Building Auth Service..."
docker build -t tourist-agency-auth:latest ./TouristAgencyAuth/TouristAgencyAuth
if [ $? -ne 0 ]; then
    echo "❌ Failed to build auth service"
    exit 1
fi

echo "  Building API Gateway..."
docker build -t tourist-agency-api:latest ./TouristAgencyAPI
if [ $? -ne 0 ]; then
    echo "❌ Failed to build API gateway"
    exit 1
fi

echo "  Building Frontend..."
docker build -t tourist-agency-ui:latest ./TouristAgencyUI
if [ $? -ne 0 ]; then
    echo "❌ Failed to build frontend"
    exit 1
fi

echo "✅ Docker images built successfully!"

# Deploy to Kubernetes
echo "🚀 Deploying to Kubernetes..."
kubectl apply -k k8s/
if [ $? -ne 0 ]; then
    echo "❌ Failed to deploy to Kubernetes"
    exit 1
fi

# Wait for deployments to be ready
echo "⏳ Waiting for deployments to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/mysql -n tourist-agency
kubectl wait --for=condition=available --timeout=300s deployment/auth-service -n tourist-agency
kubectl wait --for=condition=available --timeout=300s deployment/api-gateway -n tourist-agency
kubectl wait --for=condition=available --timeout=300s deployment/frontend -n tourist-agency

# Check deployment status
echo "📊 Checking deployment status..."
kubectl get pods -n tourist-agency
kubectl get services -n tourist-agency

# Get service URLs
echo "🌐 Service URLs:"
echo "  Frontend: $(minikube service frontend -n tourist-agency --url)"
echo "  API Gateway: $(minikube service api-gateway -n tourist-agency --url)"
echo "  Auth Service: $(minikube service auth-service -n tourist-agency --url)"

echo "✅ Setup complete! You can now access the services using the URLs above."
echo "💡 Tip: Use 'kubectl logs -f deployment/<service-name> -n tourist-agency' to view logs"