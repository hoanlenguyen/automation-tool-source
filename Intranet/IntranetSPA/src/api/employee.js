import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/employee/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/employee',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/employee/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/employee/${id}`,
    method: 'delete'})
}