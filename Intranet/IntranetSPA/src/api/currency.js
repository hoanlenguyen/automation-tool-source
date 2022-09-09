import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/currency/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/currency',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/currency/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/currency/${id}`,
    method: 'delete'})
}

export function getDropdown() {
  return request({
    url: '/currency/dropdown',
    method: 'get'
  })
}