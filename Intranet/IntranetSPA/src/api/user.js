import request from '@/utils/request'

export function login(data) {
  return request({
    url: '/auth/login',
    method: 'post',
    data})
}

export function getProfile(token) {
  return request({
    url: '/auth/getProfile',
    method: 'get'
  })
}

export function logout() {
 
}

export function changePassword(data) {
  return request({
    url: '/auth/changePassword',
    method: 'post',
    data})
}

