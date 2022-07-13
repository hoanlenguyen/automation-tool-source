import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/brand/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/brand',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/brand/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/brand/${id}`,
    method: 'delete'})
}