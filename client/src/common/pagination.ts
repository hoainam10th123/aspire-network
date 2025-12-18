export interface IPagination<T> {
    totalRecords: number
    pageNumber: number
    pageSize: number
    totalPages: number
    data: T[]
}