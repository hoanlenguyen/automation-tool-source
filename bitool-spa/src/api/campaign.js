import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/campaign/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/campaign',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/campaign/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/campaign/${id}`,
    method: 'delete'})
}

export function getDropdown() {
  return request({
    url: '/campaign/dropdown',
    method: 'get'
  })
}