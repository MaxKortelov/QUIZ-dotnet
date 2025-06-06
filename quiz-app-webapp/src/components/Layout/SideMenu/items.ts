import {useNavigate} from "react-router-dom";
import {URL_HOME} from "utils/constants/clientUrl";

export enum SideMenuItemsEnum {
    DASHBOARD = 'DASHBOARD',
}

export type SideMenuItemType = {
    label: string,
    value?: SideMenuItemsEnum,
    className?: string,
    onClick: () => void,
    disabled?: boolean,
}

export const sideMenuItems = () => {
    // eslint-disable-next-line react-hooks/rules-of-hooks
    const navigate = useNavigate();

    const profile = {
        label: 'Dashboard',
        value: SideMenuItemsEnum.DASHBOARD,
        onClick: () => {
            navigate(URL_HOME.path());
        },
    } as SideMenuItemType;

    return [profile] as SideMenuItemType[];
};
