"use client"

import * as React from 'react';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import Divider from '@mui/material/Divider';
import ListItemText from '@mui/material/ListItemText';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import Avatar from '@mui/material/Avatar';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import usePostStore from '@/stores/postStore';
import { useShallow } from 'zustand/react/shallow'
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import agent from '@/lib/agent';
import { IPost } from '@/models/post';


export default function ListCommentDetail({ post }: { post: IPost }) {   
    const setPost = usePostStore(state => state.setPost); 
    const comments = usePostStore(useShallow(state => state.pagination?.data
        .find(p => p.id === post.id)?.comments || []));

    const [content, setContent] = React.useState("")

    const handleOnChange = async (e: any) => {
        setContent(e.target.value)
    }

    const addComment = async () => {
        // call api se realtime comment qua signalR
        await agent.Posts.addComment(post.id, content)
    }

    React.useEffect(() => {
        setPost(post)
    }, [post])

    return (
        <List sx={{ width: '100%', maxWidth: 400, bgcolor: 'background.paper' }}>
            {
                comments.map((comment, index) => (
                    <Box key={`${comment.id}-${index}`}>
                        <ListItem alignItems="flex-start">
                            <ListItemAvatar>
                                <Avatar alt="Remy Sharp" src="/user.png" />
                            </ListItemAvatar>
                            <ListItemText
                                primary={comment.user.fullName}
                                secondary={
                                    <React.Fragment>
                                        <Typography
                                            component="span"
                                            variant="body2"
                                            sx={{ color: 'text.primary', display: 'inline' }}
                                        >
                                            {new Date(comment.createdAt).toLocaleDateString() + " " + new Date(comment.createdAt).toLocaleTimeString()}
                                        </Typography>
                                        {` — ${comment.content}`}
                                    </React.Fragment>
                                }
                            />
                        </ListItem>
                        <Divider variant="inset" component="li" />
                    </Box>
                ))
            }

            <Box sx={{ display: 'flex', flexDirection: 'row', gap: 1, padding: 2 }}>
                <TextField id="outlined-basic"
                    fullWidth
                    label="Outlined"
                    variant="outlined"
                    value={content}
                    onChange={handleOnChange} />
                    
                <Button variant='contained' onClick={addComment}>Add</Button>
            </Box>

        </List>
    )
}