import React, { useState, useContext, useEffect } from "react";
import { ApiContext } from "src/infrastructure/ApiContextProvider";
import { Avatar, Typography, TextField, Button, Box, makeStyles, Container } from "@material-ui/core";
import LockOutlinedIcon from "@material-ui/icons/LockOutlined";
import Alert from "@material-ui/lab/Alert";

interface IProps {
    login: any;
}

interface IState {
    userName: string;
    password: string;
}

const useStyles = makeStyles((theme) => ({
    paper: {
        marginTop: theme.spacing(8),
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
    },
    avatar: {
        margin: theme.spacing(1),
        backgroundColor: theme.palette.secondary.main,
    },
    form: {
        width: "100%", // Fix IE 11 issue.
        marginTop: theme.spacing(1),
    },
    submit: {
        margin: theme.spacing(3, 0, 2),
    },
}));

export interface ILoginResult {
    access_token: string;
    refresh_token: string;
    espires_in: number;
}

function Copyright() {
    return (
        <Typography variant="body2" color="textSecondary" align="center">
            {"Copyright © "}
            Boilerplate {new Date().getFullYear()}
            {"."}
        </Typography>
    );
}

export const Login = (props: IProps) => {
    const classes = useStyles();
    const apiContext = useContext(ApiContext);
    const [isButtonDisabled, setIsButtonDisabled] = useState(true);
    const [errorMessage, setErrorMessage] = useState("");
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");

    useEffect(() => {
        if (userName.trim() && password.trim()) {
            setIsButtonDisabled(false);
        } else {
            setIsButtonDisabled(true);
        }
    }, [userName, password]);

    const login = async (e: any) => {
        setErrorMessage("");
        e.preventDefault();
        const result = await apiContext.post<ILoginResult>("jwt", { userName, password });
        if (result) {
            props.login(result);
        } else {
            setErrorMessage("Unknown username or bad password");
        }
    };

    return (
        <Container component="main" maxWidth="xs">
            <div className={classes.paper}>
                <Avatar className={classes.avatar}>
                    <LockOutlinedIcon />
                </Avatar>
                <Typography component="h1" variant="h5">
                    Sign in
                </Typography>

                <form className={classes.form} noValidate>
                    <TextField
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        id="email"
                        label="Email Address"
                        name="email"
                        autoComplete="email"
                        autoFocus
                        onChange={(e) => setUserName(e.target.value)}
                    />
                    <TextField
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        name="password"
                        label="Password"
                        type="password"
                        id="password"
                        autoComplete="current-password"
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    {/* <FormControlLabel control={<Checkbox value="remember" color="primary" />} label="Remember me" /> */}
                    <Button type="submit" fullWidth variant="contained" color="primary" className={classes.submit} onClick={login} disabled={isButtonDisabled}>
                        Sign In
                    </Button>
                </form>
            </div>
            {errorMessage && <Alert severity="error">{errorMessage}</Alert>}
            <Box mt={8}>
                <Copyright />
            </Box>
        </Container>
    );
};
