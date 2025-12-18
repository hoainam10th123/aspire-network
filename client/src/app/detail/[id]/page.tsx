import ListCommentDetail from '@/components/ListCommentDetail';
import { IPost } from '@/models/post';
import { headers } from 'next/headers'
import { Suspense } from 'react'
import Card from '@mui/material/Card';
import CardHeader from '@mui/material/CardHeader';
import CardContent from '@mui/material/CardContent';
import CardActions from '@mui/material/CardActions';
import Avatar from '@mui/material/Avatar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Image from 'next/image'
import MoreVertIcon from '@mui/icons-material/MoreVert';
import FavoriteIcon from '@mui/icons-material/Favorite';
import ShareIcon from '@mui/icons-material/Share';
import Button from '@mui/material/Button';
import { red } from '@mui/material/colors';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import SkeletonPost from '@/components/SkeletonPost';


export async function fetchPostById(
    postId: string,
    token: string
) {
    const urlbase = 'http://localhost:5002'
    const res = await fetch(`${urlbase}/posts/${postId}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
    });

    if (!res.ok) {
        throw new Error(`Fetch post failed: ${res.status}`);
    }

    const data = await res.json()
    return data as IPost;
}


export default async function Page({
    params,
}: {
    params: Promise<{ id: string }>
}) {
    const requestHeaders = await headers();
    const token = requestHeaders.get('x-token');
    const { id } = await params

    if (!token) {
        return <p style={{color: 'red'}}>Access Denied</p>
    }

    return (
        <Suspense fallback={<SkeletonPost />}>
            <DetailPage postId={id} token={token} />
        </Suspense>
    )
}


async function DetailPage({ postId, token }: { postId: string, token: string }) {
    const post = await fetchPostById(postId, token)

    return (
        <Box sx={{ flexGrow: 1 }}>
            <Grid container spacing={2}>
                <Grid size={3}>

                </Grid>
                <Grid size={6}>
                    <Card sx={{}}>
                        <CardHeader
                            avatar={
                                <Avatar sx={{ bgcolor: red[500] }} aria-label="recipe">
                                    R
                                </Avatar>
                            }
                            action={
                                <IconButton aria-label="settings">
                                    <MoreVertIcon />
                                </IconButton>
                            }
                            title={post.user?.fullName}
                            subheader={new Date(post.createdAt).toLocaleDateString() + " " + new Date(post.createdAt).toLocaleTimeString()}
                        />

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
                        {/* <CardActions disableSpacing>
                <IconButton aria-label="add to favorites" onClick={handleLike}>
                    <FavoriteIcon color={like ? 'secondary' : 'inherit'} />
                    {
                        post.totalLike > 0 ? <div style={{ color: 'green' }}>{post.totalLike}</div> : <></>
                    }
                </IconButton>
                <IconButton aria-label="share">
                    <ShareIcon />
                </IconButton>

                <Button size="small" onClick={() => router.push(`/detail/${post.id}`)}>View</Button>

            </CardActions> */}

                        <ListCommentDetail post={post} />
                    </Card>
                </Grid>
                <Grid size={3}>

                </Grid>
            </Grid>
        </Box>
    )
}