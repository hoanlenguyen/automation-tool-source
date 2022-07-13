import request from '@/utils/request'

export function getSummary() {
  return request({
    url: '/sourceReport/getSummary',
    method: 'get'              
  })
}

