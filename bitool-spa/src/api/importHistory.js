import request from '@/utils/request'

export function getPagingImportHistories(input) {
  return request({
    url: '/importHistory/paging',
    method: 'post',
    data: input
  })
}

export function getSource() {
  return request({
    url: '/importHistory/getSource',
    method: 'get'
  })
}

