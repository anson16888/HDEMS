/**
 * Cryptographic utilities for Mock authentication
 * Uses Web Crypto API for SHA256 hashing
 */

/**
 * Hash a string using SHA256
 * @param {string} text - Plain text to hash
 * @returns {Promise<string>} Hex-encoded hash
 */
export async function sha256(text) {
  if (!window.crypto || !window.crypto.subtle) {
    throw new Error('Web Crypto API not supported in this browser')
  }

  const encoder = new TextEncoder()
  const data = encoder.encode(text)
  const hashBuffer = await window.crypto.subtle.digest('SHA-256', data)

  // Convert buffer to hex string
  const hashArray = Array.from(new Uint8Array(hashBuffer))
  const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join('')

  return hashHex
}

/**
 * Generate a random hex string
 * @param {number} length - Length of hex string (default: 32)
 * @returns {string} Random hex string
 */
export function generateRandomHex(length = 32) {
  const chars = '0123456789abcdef'
  let result = ''
  for (let i = 0; i < length; i++) {
    result += chars[Math.floor(Math.random() * chars.length)]
  }
  return result
}

/**
 * Simulate a JWT token (not cryptographically valid)
 * @param {object} payload - Token payload
 * @returns {string} Simulated JWT string
 */
export function generateSimulatedJWT(payload) {
  const header = {
    alg: 'HS256',
    typ: 'JWT'
  }

  const now = Math.floor(Date.now() / 1000)
  const tokenPayload = {
    ...payload,
    iat: now,
    exp: now + (8 * 60 * 60), // 8 hours
    refresh_exp: now + (7 * 60 * 60) // Can refresh until 1 hour before expiry
  }

  // Simulate JWT encoding (base64url)
  const encodedHeader = btoa(JSON.stringify(header))
    .replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '')
  const encodedPayload = btoa(JSON.stringify(tokenPayload))
    .replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '')
  const signature = generateRandomHex(32) // Fake signature

  return `${encodedHeader}.${encodedPayload}.${signature}`
}

/**
 * Parse a simulated JWT token
 * @param {string} token - JWT token string
 * @returns {object|null} Parsed payload or null
 */
export function parseSimulatedJWT(token) {
  try {
    const parts = token.split('.')
    if (parts.length !== 3) return null

    const payload = parts[1]
    const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
    return JSON.parse(decoded)
  } catch {
    return null
  }
}

/**
 * Check if token is expired
 * @param {object} payload - Parsed JWT payload
 * @returns {boolean}
 */
export function isTokenExpired(payload) {
  if (!payload || !payload.exp) return true
  const now = Math.floor(Date.now() / 1000)
  return now >= payload.exp
}

/**
 * Check if token can be refreshed (within 1 hour of expiry)
 * @param {object} payload - Parsed JWT payload
 * @returns {boolean}
 */
export function canRefreshToken(payload) {
  if (!payload || !payload.refresh_exp) return false
  const now = Math.floor(Date.now() / 1000)
  return now < payload.refresh_exp
}

/**
 * Get token expiry time in seconds
 * @param {object} payload - Parsed JWT payload
 * @returns {number} Seconds until expiry (0 if expired)
 */
export function getTokenExpiresIn(payload) {
  if (!payload || !payload.exp) return 0
  const now = Math.floor(Date.now() / 1000)
  const expiresIn = payload.exp - now
  return Math.max(0, expiresIn)
}
