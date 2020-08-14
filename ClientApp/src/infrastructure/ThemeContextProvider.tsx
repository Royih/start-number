import React, { useState } from "react";
import { createMuiTheme, ThemeProvider, ThemeOptions } from "@material-ui/core";
import { useLocalStorage } from "./UseLocalStorageForBoolean";
import { green } from "@material-ui/core/colors";

const themeObject = {
    palette: {
        primary: {
            // light: will be calculated from palette.primary.main,
            main: green[800],

            // dark: will be calculated from palette.primary.main,
            // contrastText: will be calculated to contrast with palette.primary.main
        },
        secondary: {
            //light: "#0066ff",
            main: "#f44336",
            // dark: will be calculated from palette.secondary.main,
            //contrastText: "#ffcc00"
        },
        // Used by `getContrastText()` to maximize the contrast between
        // the background and the text.
        contrastThreshold: 3,
        // Used by the functions below to shift a color's luminance by approximately
        // two indexes within its tonal palette.
        // E.g., shift from Red 500 to Red 300 or Red 700.
        tonalOffset: 0.2,
        type: "light",
    },
};

export type ThemeContextState = {
    toggleMode(): void;
};

const ThemeContext = React.createContext({} as ThemeContextState); // Create a context object

export { ThemeContext };

export const ThemeContextProvider = (props: any) => {
    const [value, updateValue] = useLocalStorage("darkMode", false);
    themeObject.palette.type = value ? "dark" : "light";
    const [theme, setTheme] = useState(themeObject);
    return (
        <ThemeContext.Provider
            value={{
                toggleMode: () => {
                    const {
                        palette: { type },
                    } = theme;
                    const updatedTheme = { ...theme, palette: { ...theme.palette, type: type === "light" ? "dark" : "light" } };
                    updateValue(updatedTheme.palette.type === "dark");
                    setTheme(updatedTheme);
                },
            }}
        >
            <ThemeProvider theme={createMuiTheme(theme as ThemeOptions)}>{props.children}</ThemeProvider>
        </ThemeContext.Provider>
    );
};
