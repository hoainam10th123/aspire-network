import { IPagination } from "@/common/pagination";
import { IComment, IPost } from "@/models/post";
import { instance } from "@/providers/AxiosInterceptor";
import axios, { AxiosResponse } from "axios";


const responseBody = <T>(response: AxiosResponse<T>) => response.data;


const requests = {
    get: <T>(url: string, token?: string, params?: URLSearchParams) => instance.get<T>(url, { params, headers: { "Authorization": `Bearer ${token}` } }).then(responseBody),
    post: <T>(url: string, body: {}) => instance.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => instance.put<T>(url, body).then(responseBody),
    delete: <T>(url: string) => instance.delete<T>(url).then(responseBody),
    upload: <T>(url: string, body: {}, onUploadProgress: any) => instance.post<T>(url, body, {
        headers: {
            "Content-Type": "multipart/form-data",
        },
        onUploadProgress,
    }).then(responseBody),
}

const Error = {
    notfound: () => requests.get<any>('/error/NotFound'),
    badRequest: () => requests.get<any>('/error/BadRequest'),
    authorized: () => requests.get<any>('/error/Unauthorized'),
    forbidden: () => requests.get<any>('/error/Forbidden'),
    serverError: () => requests.get<any>('/error/ServerError'),
    validationError: () => requests.post<any>('/error/Validation', {}),
}

const Posts = {
    like: (postId: string) => requests.put<{msg : string, isLike : boolean, userId : string}>(`/posts/like/${postId}`, {}),
    getPosts: (token: string, 
        pageNumber: number, 
        pageSize: number) => requests.get<IPagination<IPost>>(`/posts?pageNumber=${pageNumber}&pageSize=${pageSize}`, token),
    addComment: (postId: string, content: string) => requests.post<IComment>(`/posts/${postId}/comments`, {content}),
    addPost: (formData: FormData) => requests.post<IPost>(`/posts`, formData),
    deletePost: (postId: string) => requests.delete<{postId: string}>(`/posts/${postId}`),
}



const agent = {
    Error, Posts
}

export default agent;