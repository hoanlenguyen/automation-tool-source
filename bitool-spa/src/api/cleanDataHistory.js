import request from '@/utils/request'

export function getPaging(input) {
  return request({
    url: '/cleanDataHistory/paging',
    method: 'post',
    data: input
  })
}

export function getSource() {
  return request({
    url: '/cleanDataHistory/getSource',
    method: 'get'
  })
}

