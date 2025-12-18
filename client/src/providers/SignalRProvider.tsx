"use client"

import { createContext, useContext, useEffect, useRef } from "react";
import { createStore, useStore, StoreApi } from 'zustand'
import { useSession } from "next-auth/react";
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import usePostStore from "@/stores/postStore";
import { IComment } from "@/models/post";



interface SocketState {
    socket: HubConnection | null;
    setSignalR: (signalr: HubConnection | null) => void;
}

const SocketContext = createContext<StoreApi<SocketState> | null>(null);


export const SocketProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { data: session } = useSession()
    const { typing, addComment } = usePostStore.getState()
    const storeRef = useRef<StoreApi<SocketState> | null>(null);

    if (storeRef.current === null) {
        storeRef.current = createStore<SocketState>((set) => ({
            socket: null,
            setSignalR: (socket) => set({ socket }),
        }))
    }

    useEffect(() => {
        if (session) {
            const { setSignalR } = storeRef.current!.getState();

            const hubConnection = new HubConnectionBuilder()
                .withUrl('http://localhost:5002/hub/chat')
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Information)
                .build();

            hubConnection.start().catch(error => console.log('Error establishing the connection: ', error));

            hubConnection.on('Typing', (postId: string, data: boolean) => {
                console.log(`User is typing: ${postId}`, data);
                typing(postId, data)
            })

            hubConnection.on('AddComment', (comment: IComment) => {
                console.log(`AddComment:`, comment);
                addComment(comment)
            })

            setSignalR(hubConnection)
        }
    }, [session])

    return (
        <SocketContext.Provider value={storeRef.current}>
            {children}
        </SocketContext.Provider>
    )
}

export const useSignalR = () => {
    const store = useContext(SocketContext)
    if (!store) {
        throw new Error('Missing StoreProvider')
    }
    return useStore(store)
}