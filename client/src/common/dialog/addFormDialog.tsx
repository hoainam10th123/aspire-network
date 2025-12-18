import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import agent from '@/lib/agent';
import usePostStore from '@/stores/postStore';


export default function AddPostFormDialog() {
    const {toogleFormDialog, showFormDialog, addPost} = usePostStore(state => state);
    const [title, setTitle] = React.useState("");
    const [content, setContent] = React.useState("");
    const [formData, setFormData] = React.useState<FormData>(new FormData());
    const [file, setFile] = React.useState<File | null>(null);

    const handleClickOpen = () => {
        toogleFormDialog(true);
    };

    const handleClose = () => {
        toogleFormDialog(false);
    };

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const formData = new FormData(event.currentTarget);
        const formJson = Object.fromEntries((formData as any).entries());
        //const title = formJson.title;
        //const content = formJson.content;
        formData.append('json', JSON.stringify({ Title: title, Content: content }));
        if(file) {
            formData.append('file', file);
        }

        const res = await agent.Posts.addPost(formData)
        addPost(res);
        
        setTimeout(() => {
            handleClose();
        }, 300)
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        formData.delete('file');
        formData.delete('json');

        if (file) {
            setFile(file);
        }        
    }

    return (
        <React.Fragment>
            <Button variant="outlined" onClick={handleClickOpen}>
                Open form dialog
            </Button>
            <Dialog open={showFormDialog} onClose={handleClose}>
                <DialogTitle>Subscribe</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        To subscribe to this website, please enter your email address here. We
                        will send updates occasionally.
                    </DialogContentText>
                    <form onSubmit={handleSubmit} id="subscription-form">
                        <TextField
                            autoFocus
                            required
                            margin="dense"
                            id="title"
                            name="title"
                            label="Title"
                            type="text"
                            fullWidth
                            variant="standard"
                            value={title}
                            onChange={e=> setTitle(e.target.value)}
                        />

                        <TextField
                            autoFocus
                            required
                            margin="dense"
                            id="content"
                            name="content"
                            label="Content"
                            type="text"
                            fullWidth
                            variant="standard"
                            value={content}
                            onChange={e=> setContent(e.target.value)}
                        />
                    </form>

                    {
                        file && <div style={{color: 'green'}}>{file.name}</div>
                    }
                    <input
                        type="file"
                        id="file"
                        hidden
                        onChange={handleChange}
                        accept=".png,.jpg,.jpeg"
                    />
                    <Button onClick={() => document.getElementById('file')?.click()}>
                        Upload file
                    </Button>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button type="submit" form="subscription-form">
                        Save
                    </Button>
                </DialogActions>
            </Dialog>
        </React.Fragment>
    );
}
