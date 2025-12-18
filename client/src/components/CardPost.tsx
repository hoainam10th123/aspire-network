'use client';

import * as React from 'react';
import Card from '@mui/material/Card';
import CardHeader from '@mui/material/CardHeader';
import CardContent from '@mui/material/CardContent';
import CardActions from '@mui/material/CardActions';
import Avatar from '@mui/material/Avatar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import { red } from '@mui/material/colors';
import FavoriteIcon from '@mui/icons-material/Favorite';
import ShareIcon from '@mui/icons-material/Share';
import { IPost } from '@/models/post';
import Image from 'next/image'
import agent from '@/lib/agent';
import usePostStore from '@/stores/postStore';
import TextField from '@mui/material/TextField';
import { useSignalR } from '@/providers/SignalRProvider';
import Button from '@mui/material/Button';
import ListComment from './ListComment';
import { useRouter } from 'next/navigation'
import DeleteIcon from '@mui/icons-material/Delete';


interface Props {
    post: IPost
}

export default function CardPost({ post }: Props) {
    const { like: clickLike, delPost: del } = usePostStore(state => state)
    const [like, setLike] = React.useState(post.likedByMe)
    const [content, setContent] = React.useState("")
    const { socket } = useSignalR()
    const router = useRouter()

    const handleLike = async () => {
        const res = await agent.Posts.like(post.id)
        if (res.isLike) {
            clickLike(post.id, true)
            setLike(true)
        }
        else {
            clickLike(post.id, false)
            setLike(false)
        }
    }

    const handleOnChange = async (e: any) => {
        setContent(e.target.value)
        const lastTypeing = new Date().getTime()
        if (!post.typing) {
            try {
                await socket?.invoke("Typing", post.id)
            } catch (err: any) {
                console.error(err)
            }
        }

        const timer = 2000
        setTimeout(() => {
            const timeNow = new Date().getTime()
            const timeDiff = timeNow - lastTypeing

            if (timeDiff >= timer && post.typing) {
                socket?.invoke('StopTyping', post.id)
                    .catch((err: any) => console.error(err));
            }
        }, timer)
    }

    const addComment = async () => {
        // call api se realtime comment qua signalR
        await agent.Posts.addComment(post.id, content)
    }

    const delPost = async (postId: string) => {
        var result = confirm("Are you sure you want to delete this post?");
        if (result) {
            await agent.Posts.deletePost(postId)
            del(postId)
        }
    }

    React.useEffect(() => {
        setLike(like)
    }, [])

    return (
        <Card sx={{ borderRadius: 5, marginBottom: 2, border: '1px solid #2819fcff' }}>
            <CardHeader
                avatar={
                    <Avatar sx={{ bgcolor: red[500] }} aria-label="recipe">
                        R
                    </Avatar>
                }
                action={
                    <IconButton aria-label="settings" onClick={() => delPost(post.id)}>
                        <DeleteIcon />
                    </IconButton>
                }
                title={post.user?.fullName}
                subheader={new Date(post.createdAt).toLocaleDateString() + " " + new Date(post.createdAt).toLocaleTimeString()}
            />

            <Typography variant="h5" sx={{ color: 'text.secondary', marginLeft: 4 }}>
                {post.title}
            </Typography>
            {
                post.photos.length > 0 && <Image
                    loading='lazy'
                    src={`http://localhost:5002${post.photos[0].url}`}
                    alt={post.title}
                    height={440}
                    width={600}
                    priority={false}
                    unoptimized={true}
                />
            }

            <CardContent>
                <Typography variant="body2" sx={{ color: 'text.secondary' }}>
                    {post.content}
                </Typography>
            </CardContent>
            <CardActions disableSpacing>
                <IconButton aria-label="add to favorites" onClick={handleLike}>
                    <FavoriteIcon color={like ? 'secondary' : 'inherit'} />
                    {
                        post.totalLike > 0 ? <div style={{ color: 'green' }}>{post.totalLike}</div> : <></>
                    }
                </IconButton>
                <IconButton aria-label="share">
                    <ShareIcon />
                </IconButton>

                <TextField id="outlined-basic"
                    fullWidth
                    label="Outlined"
                    variant="outlined"
                    value={content}
                    onChange={handleOnChange} />

                <Button variant='text' onClick={addComment}>Add</Button>
                <Button size="small" onClick={() => router.push(`/detail/${post.id}`)}>View</Button>

            </CardActions>
            {
                post.typing && <div style={{
                    borderRadius: 10,
                    backgroundColor: 'rgba(131, 255, 238, 0.28)',
                    padding: 10, margin: 10
                }}>
                    typing...
                </div>
            }

            <ListComment comments={post.comments} />
        </Card>
    );
}
