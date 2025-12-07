# Minikube Setup Guide

This guide will help you deploy the Tourist Agency microservices to Minikube for local development.

## Prerequisites

1. **Minikube installed and running**
   ```bash
   minikube start
   minikube status
   ```

2. **Docker configured with Minikube**
   ```bash
   eval $(minikube docker-env)
   ```

3. **kubectl configured**
   ```bash
   kubectl config current-context
   # Should show: minikube
   ```

## Step 1: Build Docker Images

Since we're using local images, we need to build them in the Minikube Docker environment:

```bash
# Set Docker environment to Minikube
eval $(minikube docker-env)

# Build images
docker build -t tourist-agency-auth:latest ./services/auth
docker build -t tourist-agency-api:latest ./services/api
docker build -t tourist-agency-ui:latest ./frontend

# Verify images are built
docker images | grep tourist-agency
```

## Step 2: Deploy to Minikube

```bash
# Apply all Kubernetes manifests
kubectl apply -k k8s/

# Check deployment status
kubectl get pods -n tourist-agency
kubectl get services -n tourist-agency
```

## Step 3: Access Services

### Option 1: Port Forwarding (Recommended for Development)

```bash
# Terminal 1 - Auth Service
kubectl port-forward service/auth-service 7002:7002 -n tourist-agency

# Terminal 2 - API Gateway
kubectl port-forward service/api-gateway 7001:80 -n tourist-agency

# Terminal 3 - Frontend
kubectl port-forward service/frontend 3000:80 -n tourist-agency
```

Then access:
- Frontend: http://localhost:3000
- API Gateway: http://localhost:7001
- Auth Service: http://localhost:7002

### Option 2: Minikube Service URLs

```bash
# Get service URLs
minikube service frontend -n tourist-agency --url
minikube service api-gateway -n tourist-agency --url
minikube service auth-service -n tourist-agency --url
```

### Option 3: Ingress (Advanced)

If you want to use the ingress, you need to enable the ingress addon:

```bash
# Enable ingress addon
minikube addons enable ingress

# Add to hosts file (Windows: C:\Windows\System32\drivers\etc\hosts)
# Add this line:
# <minikube-ip> tourist-agency.local

# Get minikube IP
minikube ip

# Access via ingress
# http://tourist-agency.local
```

## Step 4: Verify Deployment

```bash
# Check all pods are running
kubectl get pods -n tourist-agency

# Check services
kubectl get services -n tourist-agency

# Check logs if needed
kubectl logs -f deployment/auth-service -n tourist-agency
kubectl logs -f deployment/api-gateway -n tourist-agency
kubectl logs -f deployment/frontend -n tourist-agency
```

## Troubleshooting

### Common Issues

1. **Images not found**
   ```bash
   # Make sure you're using Minikube Docker environment
   eval $(minikube docker-env)
   docker images | grep tourist-agency
   ```

2. **Pods stuck in ImagePullBackOff**
   ```bash
   # Check image names in deployment files
   kubectl describe pod <pod-name> -n tourist-agency
   ```

3. **Services not accessible**
   ```bash
   # Check service endpoints
   kubectl get endpoints -n tourist-agency
   
   # Check pod logs
   kubectl logs <pod-name> -n tourist-agency
   ```

4. **Database connection issues**
   ```bash
   # Check MySQL pod
   kubectl logs -f deployment/mysql -n tourist-agency
   
   # Test database connection
   kubectl exec -it deployment/mysql -n tourist-agency -- mysql -u tourist_user -p tourist_agency
   ```

### Debug Commands

```bash
# Get detailed pod information
kubectl describe pod <pod-name> -n tourist-agency

# Check resource usage
kubectl top pods -n tourist-agency

# Check events
kubectl get events -n tourist-agency --sort-by='.lastTimestamp'

# Access pod shell
kubectl exec -it <pod-name> -n tourist-agency -- /bin/sh
```

## Cleanup

```bash
# Delete all resources
kubectl delete namespace tourist-agency

# Or delete specific resources
kubectl delete -k k8s/
```

## Development Workflow

1. **Make code changes**
2. **Rebuild images**
   ```bash
   eval $(minikube docker-env)
   docker build -t tourist-agency-auth:latest ./services/auth
   docker build -t tourist-agency-api:latest ./services/api
   docker build -t tourist-agency-ui:latest ./frontend
   ```
3. **Restart deployments**
   ```bash
   kubectl rollout restart deployment/auth-service -n tourist-agency
   kubectl rollout restart deployment/api-gateway -n tourist-agency
   kubectl rollout restart deployment/frontend -n tourist-agency
   ```

## Performance Tips

1. **Increase Minikube resources**
   ```bash
   minikube start --memory=4096 --cpus=2
   ```

2. **Use local registry (optional)**
   ```bash
   minikube addons enable registry
   ```

3. **Enable metrics server**
   ```bash
   minikube addons enable metrics-server
   ```