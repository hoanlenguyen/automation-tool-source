import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/bank/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/bank',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/bank/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/bank/${id}`,
    method: 'delete'})
}

export function getDropdown() {
  return request({
    url: '/bank/dropdown',
    method: 'get'
  })
}