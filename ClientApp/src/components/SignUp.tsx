import React, { useState, useContext } from "react";
import Avatar from "@material-ui/core/Avatar";
import Button from "@material-ui/core/Button";
import CssBaseline from "@material-ui/core/CssBaseline";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";
import Link from "@material-ui/core/Link";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import LockOutlinedIcon from "@material-ui/icons/LockOutlined";
import Typography from "@material-ui/core/Typography";
import { makeStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";
import { useParams } from "react-router";
import { SignUpDto, CommandResultDto } from "src/models";
import { ValidationElement, FormValidator } from "src/infrastructure/validator";
import { useSnackbar } from "notistack";
import { ApiContext } from "src/infrastructure/ApiContextProvider";

function Copyright() {
  return (
    <Typography variant="body2" color="textSecondary" align="center">
      {"Copyright © "}
      <Link color="inherit" href="https://material-ui.com/">
        Signup!
      </Link>{" "}
      {new Date().getFullYear()}
      {"."}
    </Typography>
  );
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
    marginTop: theme.spacing(3),
  },
  submit: {
    margin: theme.spacing(3, 0, 2),
  },
}));

export const SignUp = () => {
  const api = useContext(ApiContext);
  const classes = useStyles();
  const { key } = useParams();

  const emptyForm = { tenantKey: key, firstName: "", surName: "", email: "", allowUsToContactPersonByEmail: true, previouslyParticipated: false } as SignUpDto;
  const [signUpData, setSignUpData] = useState({ ...emptyForm });

  const validatorElements = [];
  validatorElements.push(new ValidationElement("firstName", signUpData.firstName, { required: true, minLength: 2, maxLength: 100 }));
  validatorElements.push(new ValidationElement("surName", signUpData.surName, { required: true, minLength: 2, maxLength: 100 }));
  validatorElements.push(new ValidationElement("email", signUpData.email, { required: true, email: true }));

  const [validator, setValidator] = useState(new FormValidator(validatorElements));

  const { enqueueSnackbar } = useSnackbar();

  const handleTextChange = (key: string, newValue: string) => {
    const newState = { ...signUpData, [key]: newValue } as SignUpDto;
    setSignUpData(newState);
    setValidator(validator.updateValue(key, newValue));
  };

  const handleBoolChange = (key: string, newValue: boolean) => {
    const newState = { ...signUpData, [key]: newValue } as SignUpDto;
    setSignUpData(newState);
    setValidator(validator.updateValue(key, newValue));
  };

  const handleBlur = (name: string) => {
    setValidator(validator.onBlur(name));
  };

  const save = async () => {
    const saveResult = await api.post<CommandResultDto<any>>("anonymous/signUp", { signUpData: signUpData });
    if (saveResult && saveResult.success) {
      enqueueSnackbar("Data submitted", { variant: "success", anchorOrigin: { vertical: "top", horizontal: "right" } });
      setSignUpData({ ...emptyForm });
      setValidator(validator.setPristine());
    } else {
      enqueueSnackbar("Error occurred: " + saveResult?.errorMessages?.join(", ") || "", { variant: "error", anchorOrigin: { vertical: "bottom", horizontal: "center" } });
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <div className={classes.paper}>
        <Avatar className={classes.avatar}>
          <LockOutlinedIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Sign up to {key}
        </Typography>
        <form action="" autoComplete="off" className={classes.form} noValidate>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <TextField
                variant="outlined"
                fullWidth
                autoFocus
                id="signup-firstName"
                label="First name"
                value={signUpData.firstName}
                disabled={false}
                required={true}
                autoComplete="fname"
                error={validator.reportError("firstName")}
                helperText={validator.errorMessage("firstName")}
                onChange={(event) => handleTextChange("firstName", event.target.value)}
                onBlur={() => handleBlur("firstName")}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                variant="outlined"
                fullWidth
                id="signup-surName"
                label="Last name"
                value={signUpData.surName}
                disabled={false}
                required={true}
                autoComplete="lname"
                error={validator.reportError("surName")}
                helperText={validator.errorMessage("surName")}
                onChange={(event) => handleTextChange("surName", event.target.value)}
                onBlur={() => handleBlur("surName")}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                variant="outlined"
                fullWidth
                id="signup-email"
                label="Email address"
                value={signUpData.email}
                disabled={false}
                required={true}
                autoComplete="email"
                error={validator.reportError("email")}
                helperText={validator.errorMessage("email")}
                onChange={(event) => handleTextChange("email", event.target.value)}
                onBlur={() => handleBlur("email")}
              />
            </Grid>
            <Grid item xs={12}>
              <FormControlLabel
                control={
                  <Checkbox
                    value="allowUsToContactPersonByEmail"
                    checked={signUpData.allowUsToContactPersonByEmail}
                    onChange={() => handleBoolChange("allowUsToContactPersonByEmail", !signUpData.allowUsToContactPersonByEmail)}
                    color="primary"
                  />
                }
                label="I can be contacted on this email-address"
              />
            </Grid>
            <Grid item xs={12}>
              <FormControlLabel
                control={
                  <Checkbox
                    value="previouslyParticipated"
                    checked={signUpData.previouslyParticipated}
                    onChange={() => handleBoolChange("previouslyParticipated", !signUpData.previouslyParticipated)}
                    color="primary"
                  />
                }
                label="I have previously participated"
              />
            </Grid>
          </Grid>
          <Button type="button" fullWidth variant="contained" color="primary" className={classes.submit} onClick={save} disabled={!validator.allowSave}>
            Sign Up!
          </Button>
        </form>
        {/* <pre>{JSON.stringify(signUpData, null, 4)}</pre> */}
      </div>
      <Box mt={5}>
        <Copyright />
      </Box>
    </Container>
  );
};