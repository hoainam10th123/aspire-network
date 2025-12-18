'use client'

import Grid from '@mui/material/Grid';
import { useSession } from 'next-auth/react';
import { useEffect } from 'react';
import agent from '@/lib/agent';
import usePostStore from '@/stores/postStore';
import Fab from '@mui/material/Fab';
import AddIcon from '@mui/icons-material/Add';
import Box from '@mui/material/Box';
import dynamic from 'next/dynamic'


const CardPost = dynamic(() => import('./CardPost'));
const AddPostFormDialog = dynamic(() => import('@/common/dialog/addFormDialog'), { ssr: false });

export default function ListPost() {
    const { data } = useSession();
    const {showFormDialog, toogleFormDialog} = usePostStore(state => state);
    const { pagination, setPagination } = usePostStore(state => state);

    useEffect(() => {
        if (data) {
            agent.Posts.getPosts(data.accessToken!, 1, 10).then(data => {
                setPagination(data);
            }).catch(error => {
                console.error("Error fetching posts:", error);
            });
        }
    }, [data])


    return (
        <Grid container spacing={2} sx={{position: 'relative'}}>
            {
                pagination?.data.map(post => <Grid key={post.id} size={6}>
                    <CardPost post={post} />
                </Grid>)
            }

            <Box sx={{ position: 'fixed', bottom: 16, right: 30 }}>
                <Fab color="primary" aria-label="add" onClick={()=>toogleFormDialog(true)}>
                    <AddIcon />
                </Fab>
            </Box>

            {showFormDialog && <AddPostFormDialog />}
        </Grid>
    )
}