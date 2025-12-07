#!/bin/bash

# Tourist Agency Minikube Cleanup Script
# Run this script from the root directory (agency-api)

echo "🧹 Cleaning up Tourist Agency from Minikube..."

# Delete all resources
echo "🗑️ Deleting Kubernetes resources..."
kubectl delete -k k8s/

# Wait for resources to be deleted
echo "⏳ Waiting for resources to be deleted..."
kubectl wait --for=delete namespace/tourist-agency --timeout=60s

# Check if namespace is deleted
if kubectl get namespace tourist-agency >/dev/null 2>&1; then
    echo "⚠️ Namespace still exists, forcing deletion..."
    kubectl delete namespace tourist-agency --force --grace-period=0
fi

# Remove Docker images (optional)
read -p "Do you want to remove Docker images? (y/N): " removeImages
if [[ $removeImages =~ ^[Yy]$ ]]; then
    echo "🐳 Removing Docker images..."
    eval $(minikube docker-env)
    docker rmi tourist-agency-auth:latest 2>/dev/null
    docker rmi tourist-agency-api:latest 2>/dev/null
    docker rmi tourist-agency-ui:latest 2>/dev/null
    echo "✅ Docker images removed"
fi

echo "✅ Cleanup complete!"