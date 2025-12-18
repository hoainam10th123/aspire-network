export interface IPost {
    id: string
    title: string
    content: string
    userId: string
    createdAt: string
    lastUpdatedAt: string
    totalLike: number
    likedByMe: boolean
    comments: IComment[]        
    photos: IPhoto[]
    user: IKeycloakUser
    typing: boolean
}

export interface IComment {
    id: string
    content: string
    userId: string
    createdAt: string
    lastUpdatedAt: any
    postId: string
    user: IKeycloakUser
}

export interface IPhoto {
    id: string
    postId: string
    url: string
}

export interface IKeycloakUser {
    id: string
    username: string
    fullName: string
}