import React, { useContext } from "react";
import { makeStyles, useTheme } from "@material-ui/core/styles";
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import IconButton from "@material-ui/core/IconButton";
import LoyaltyIcon from "@material-ui/icons/Loyalty";
import { ThemeContext } from "src/infrastructure/ThemeContextProvider";
import DarkModeIcon from "@material-ui/icons/Brightness4";
import LightModeIcon from "@material-ui/icons/Brightness5";
import { useHistory } from "react-router-dom";

const useStyles = makeStyles((theme) => ({
  root: {
    flexGrow: 1,
    marginBottom: 20,
  },
  menuButton: {
    marginRight: theme.spacing(2),
  },
  title: {
    flexGrow: 1,
  },
}));

export default function NavBar() {
  const classes = useStyles();
  const theme = useTheme();
  const themeContext = useContext(ThemeContext);

  const history = useHistory();

  return (
    <div className={classes.root}>
      <AppBar position="static">
        <Toolbar>
          <IconButton
            edge="start"
            className={classes.menuButton}
            color="inherit"
            aria-label="menu"
            onClick={() => {
              history.push("/");
            }}
          >
            <LoyaltyIcon />
          </IconButton>
          <Typography variant="h6" className={classes.title}>
            Signup!
          </Typography>
          <IconButton onClick={themeContext.toggleMode}>
            {theme.palette.type === "light" && <LightModeIcon />}
            {theme.palette.type === "dark" && <DarkModeIcon />}
          </IconButton>
          <Button color="inherit">Login</Button>
        </Toolbar>
      </AppBar>
    </div>
  );
}
