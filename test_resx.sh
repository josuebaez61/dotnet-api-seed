#!/bin/bash

echo "🧪 Testing RESX Localization"
echo "=========================="

# Wait for app to start
sleep 8

# Test Spanish
echo "📝 Test: Login with non-existent user (Spanish)"
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -H "Accept-Language: es" \
  -d '{"emailOrUsername": "usuario_inexistente", "password": "password_incorrecta"}' \
  -k -s | jq .

echo -e "\n"

# Test English
echo "📝 Test: Login with non-existent user (English)"
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -H "Accept-Language: en" \
  -d '{"emailOrUsername": "usuario_inexistente", "password": "password_incorrecta"}' \
  -k -s | jq .

echo -e "\n✅ Tests completed!"
