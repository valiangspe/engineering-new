import { ctx } from "./main";

export const days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
export const makeDateString = (date) =>
  new Date(
    new Date(date).getTime() - new Date(date).getTimezoneOffset() * 60000
  )
    .toISOString()
    .split("T")[0];

export const intlFormat = (params) => {
  try {
    return Intl.DateTimeFormat("en-US", {
      dateStyle: params.dateStyle,
      timeStyle: params.timeStyle,
    }).format(new Date(params.date));
  } catch (e) {
    return "";
  }
};

export const filterActivities = (params) => {
  const filterDepartmentCond = (activity) => {
    if (!params.departmentId) {
      return true;
    }

    const mappedDeptIds = activity?.tasks
      ?.flatMap((t) => t?.inCharges)
      ?.map((ic) => ic.extUserId)
      .filter((ui) => ui)
      .map(
        (ui) =>
          ctx.value.users.find((u) => `${u?.id}` === `${ui}`)?.departmentId
      )
      .filter((di) => di);

    console.log("mappeddeptids", mappedDeptIds, params.departmentId);

    return mappedDeptIds?.find((di) => `${di}` === `${params.departmentId}`);
  };

  const filterUserCond = (activity) => {
    if (!params.userId) {
      return true;
    }

    const mappedUserIds = activity?.tasks
      ?.flatMap((t) => t?.inCharges)
      ?.map((ic) => ic.extUserId)
      .filter((ui) => ui)
      .map((ui) => ctx.value.users.find((u) => `${u?.id}` === `${ui}`)?.id)
      .filter((di) => di);

    console.log("mappedUserIds", mappedUserIds, params.userId);

    return mappedUserIds?.find((di) => `${di}` === `${params.userId}`);
  };

  const filterType = (activity) => {
    if (!params.type) {
      return true;
    }

    return activity.type === params.type;
  };

  const filterFrom = (activity) => {
    if (!params.from) {
      return true;
    }

    return (
      new Date(`${params.from}T00:00:00Z`).getTime() <=
      new Date(activity.toCache).getTime()
    );
  };

  const filterTo = (activity) => {
    if (!params.to) {
      return true;
    }

    return (
      new Date(`${params.to}T00:00:00Z`).getTime() >=
      new Date(activity.fromCache).getTime()
    );
  };

  return (
    params.activities?.filter(
      (a) =>
        filterDepartmentCond(a) &&
        filterUserCond(a) &&
        filterType(a) &&
        filterFrom(a) &&
        filterTo(a)
    ) ?? []
  );
};
