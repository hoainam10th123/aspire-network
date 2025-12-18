"use client";

import axios, { AxiosError, AxiosResponse } from 'axios';
import { useSession } from 'next-auth/react';
import { useEffect } from 'react';
import { toast } from 'react-toastify';
import { useRouter } from 'next/navigation'


const sleep = () => new Promise(resolve => setTimeout(resolve, 2000))

export const instance = axios.create({
    baseURL: 'http://localhost:5002',
})

interface Props {
    children: React.ReactNode;
}

export default function AxiosInterceptor({ children }: Props) {
    const { data: session, status } = useSession();
    //const router = useRouter()

    useEffect(() => {
        const token = session?.accessToken

        const requestInterceptor = instance.interceptors.request.use(config => {
            if (token) config.headers.Authorization = `Bearer ${token}`;
            return config;
        })

        const responseInterceptor = instance.interceptors.response.use(async response => {
            // if (process.env.NODE_ENV === 'development') await sleep();
            return response
        }, (error: AxiosError) => {
            const { status, data } = error.response as AxiosResponse;
            switch (status) {
                case 400:
                    toast.error(data.msg, { toastId: 400 });
                    break;
                case 401:
                    toast.error('401 Unauthorized', { toastId: 401 });
                    break;
                case 403:
                    toast.error('403 You are not allowed to do that!', { toastId: 403 });
                    break;
                case 404:
                    toast.error(data.msg, { toastId: 404 });
                    break;
                case 429:
                    toast.error('429 Too Many Requests', { toastId: 429 });
                    break;
                case 500:
                    //setError(data);
                    toast.error('500 server error', { toastId: 500 });
                    console.error(data)
                    //router.push('/error');
                    break;
                default:
                    console.error(data)
                    toast.error('Have a error, see console log');
                    break;
            }

            return Promise.reject(error);
        })

        return () => {
            instance.interceptors.request.eject(requestInterceptor);
            instance.interceptors.response.eject(responseInterceptor);
        };

    }, [status, session])

    return children;
}