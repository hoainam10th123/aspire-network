import * as React from 'react';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import Divider from '@mui/material/Divider';
import ListItemText from '@mui/material/ListItemText';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import Avatar from '@mui/material/Avatar';
import Typography from '@mui/material/Typography';
import { IComment } from '@/models/post';
import Box from '@mui/material/Box';


interface Props {
    comments: IComment[];
}

export default function ListComment({ comments }: Props) {
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
        </List>
    );
}
