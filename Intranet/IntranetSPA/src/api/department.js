import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/Department/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/Department',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/Department/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/Department/${id}`,
    method: 'delete'})
}

export function getDropdown() {
  return request({
    url: '/department/dropdown',
    method: 'get'
  })
}