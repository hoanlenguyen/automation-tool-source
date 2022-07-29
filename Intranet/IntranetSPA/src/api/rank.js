import request from '@/utils/request'

export function getDetail(id) {
  return request({
    url: `/Rank/${id}`,
    method: 'get'})
}

export function createOrUpdate(input) {
  return request({
    url: '/Rank',
    method:input.id==0? 'post':'put',
    data:input    
  })
}

export function getList(input) {
  return request({
    url: '/Rank/list',
    method: 'post',
    data:input    
  })
}

export function deleteData(id) {
  return request({
    url: `/Rank/${id}`,
    method: 'delete'})
}

export function getDropdown() {
  return request({
    url: '/Rank/dropdown',
    method: 'get'
  })
}