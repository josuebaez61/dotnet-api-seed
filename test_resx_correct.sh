#!/bin/bash

echo "🧪 Testing RESX Localization with Correct Port"
echo "=============================================="

# Wait for app to start
echo "⏳ Waiting for application to start..."
sleep 10

# Test Spanish
echo "📝 Test 1: Login with non-existent user (Spanish)"
echo "Expected: 'Usuario no encontrado'"
echo "---"
response=$(curl -X POST "http://localhost:5103/api/auth/login" \
  -H "Content-Type: application/json" \
  -H "Accept-Language: es" \
  -d '{"emailOrUsername": "usuario_inexistente", "password": "password_incorrecta"}' \
  -s -w "\nHTTP_STATUS:%{http_code}")

echo "$response"

# Extract HTTP status
http_status=$(echo "$response" | grep "HTTP_STATUS:" | cut -d: -f2)
echo "HTTP Status: $http_status"

echo -e "\n"

# Test English
echo "📝 Test 2: Login with non-existent user (English)"
echo "Expected: 'User not found'"
echo "---"
response2=$(curl -X POST "http://localhost:5103/api/auth/login" \
  -H "Content-Type: application/json" \
  -H "Accept-Language: en" \
  -d '{"emailOrUsername": "usuario_inexistente", "password": "password_incorrecta"}' \
  -s -w "\nHTTP_STATUS:%{http_code}")

echo "$response2"

# Extract HTTP status
http_status2=$(echo "$response2" | grep "HTTP_STATUS:" | cut -d: -f2)
echo "HTTP Status: $http_status2"

echo -e "\n"

# Test without language header (should default to English)
echo "📝 Test 3: Login with non-existent user (Default - English)"
echo "Expected: 'User not found'"
echo "---"
response3=$(curl -X POST "http://localhost:5103/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"emailOrUsername": "usuario_inexistente", "password": "password_incorrecta"}' \
  -s -w "\nHTTP_STATUS:%{http_code}")

echo "$response3"

# Extract HTTP status
http_status3=$(echo "$response3" | grep "HTTP_STATUS:" | cut -d: -f2)
echo "HTTP Status: $http_status3"

echo -e "\n✅ Tests completed!"
