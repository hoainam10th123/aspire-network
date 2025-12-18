import { IPagination } from '@/common/pagination';
import { IComment, IPost } from '@/models/post';
import { create } from 'zustand'


interface BearState {
    showFormDialog: boolean;
    pagination: IPagination<IPost> | null;
    setPagination: (data: IPagination<IPost>) => void;
    addPost: (post: IPost) => void;
    like: (postId: string, data: boolean) => void;
    typing: (postId: string, data: boolean) => void;
    addComment: (data: IComment) => void;
    setPost: (post: IPost) => void;
    toogleFormDialog: (data: boolean) => void;
    delPost: (id: string) => void;
}

const usePostStore = create<BearState>((set) => ({
    showFormDialog: false,
    pagination: null,

    setPagination: (data) => set({ pagination: data }),

    addPost: (post) =>
        set((state) => {
            const newPosts = [post, ...state.pagination!.data];
            const newPagination = {
                ...state.pagination!,
                data: newPosts,
            };
            return { pagination: newPagination };
        }),

    like: (postId, data) =>
        set((state) => {
            if (!state.pagination) return { pagination: null };

            const newData = state.pagination.data.map(post =>
                post.id === postId
                    ? { ...post, likedByMe: data } // ⭐ tạo object mới
                    : post
            );

            return {
                pagination: {
                    ...state.pagination,
                    data: newData, // ⭐ gán array mới
                },
            };
        }),

    typing: (postId, data) =>
        set((state) => {
            if (!state.pagination) return { pagination: null };

            const newData = state.pagination.data.map(post =>
                post.id === postId
                    ? { ...post, typing: data } // ⭐ tạo object mới
                    : post
            );

            return {
                pagination: {
                    ...state.pagination,
                    data: newData, // ⭐ gán array mới
                },
            };
        }),
    addComment: (data) => set((state) => {
        if (!state.pagination) return { pagination: null };

        const newData = state.pagination.data.map(post => {
            if (post.id !== data.postId) return post;

            return {
                ...post,
                comments: [...post.comments, data],  // ⭐ luôn tạo array mới
            };
        });

        return {
            pagination: {
                ...state.pagination,
                data: newData,
            },
        };
    }),
    setPost: (data) => set((state) => {
        if (!state.pagination) return state;

        return {
            pagination: {
                ...state.pagination,
                data: state.pagination.data.map(post =>
                    post.id === data.id ? data : post
                ),
            },
        };
    }),

    toogleFormDialog: (data) => set({
        showFormDialog: data,
    }),

    delPost: (id) =>
        set((state) => {
            const newPosts = state.pagination!.data.filter(post => post.id !== id);
            const newPagination = {
                ...state.pagination!,
                data: newPosts,
            };
            return { pagination: newPagination };
        }),
}));

export default usePostStore;