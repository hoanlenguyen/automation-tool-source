import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/role/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/role',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/role/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/role/${id}`,
    method: 'delete'})
}

export function getDropdown() {
  return request({
    url: '/role/dropdown',
    method: 'get'
  })
}

export function getAllPermissions() {
  return request({
    url: '/role/AllPermissions',
    method: 'get'
  })
}