using System;

public class AutoTrainMenu : mScreen, IDisposable
{
	public static bool isNhat;

	public static AutoTrainMenu instance;

	public static Paint paintUI;

	public static bool[] isChecked;

	public TField tfHpOn;

	public TField tfHpDuoi;

	public static long hpQuaiOn;

	public static long hpQuaiDuoi;

	public static bool isCheckAttackByHp;

	public static bool isCheckAutoPean;

	public static int peanHpPercent;

	public static int peanMpPercent;

	public TField tfHpPean;

	public TField tfMpPean;

	public static long lastTimeUsePean;

	public static MyVector listMobNames;

	public static bool[] isCheckedMobName;

	public static bool isCheckAllMob;

	private static int mobNameSnapshotMapId;

	private static bool mobNameSnapshotReady;

	private static bool mobSelectionPreparedForCurrentMap;

	private static bool mobSelectionLoadedFromMapRms;

	private static bool mobSelectionCurrentMapWasReloaded;

	public static MyVector listSkills;

	public static bool[] isCheckedSkill;

	public static int currentTab;

	public static bool[] isCheckItem;

	public static bool[] isCheckPick;

	public TField tfIdNhat;

	public TField tfIdLoaiTru;

	public TField tfIdVutBo;

	public static string strIdNhat;

	public static string strIdLoaiTru;

	public static string strIdVutBo;

	public TField[] tfBuaKills;

	public static int[] muaBuaKills;

	public static bool[] isCheckBua;

	public int x;

	public int y;

	public int w;

	public int h;

	public int xScroll;

	public int yScroll;

	public int wScroll;

	public int hScroll;

	public Scroll[] scrolls;

	public int selectedIndex;

	public new Command cmdClose;

	public static long lastTimeAttack;

	public static int[] ITEM_TEMPLATE_IDS;

	private static readonly long[] lastTimeUseItemByType;

	private const long ITEM_USE_COOLDOWN_MILLIS = 60000L;

	public static int[] BUA_TEMPLATE_IDS;

	public static int[] BUA_ICON_IDS;

	private static long lastTimeUseItem;

	private static long lastTimePick;

	private static long lastTimeDrop;

	private static long lastTimeBuyBua;

	private static long lastTimeChangeZone;

	private static long timeMapEmpty;

	public static long autoTrainNextZoneAllowedTime;

	public static int unreachableMobId;

	public static long unreachableMobTime;

	public static int lastTargetMobId;

	public static long targetMobStartTime;

	public static bool needRescanMobs;

	public static int lastMapIdBeforeChange;

	public static string[] OPTION_NAMES
	{
		get
		{
			return new string[3] { mResources.tuDongDanh, mResources.boQuaSieuQuai, mResources.doiKhuKhiHetQuai };
		}
	}

	public static string[] TAB_NAMES => mResources.autoTrain_TabNames;

	public static string[] TAB_NAMES_2 => mResources.autoTrain_TabNames_2;

	public static string[] ITEM_NAMES
	{
		get
		{
			if (ITEM_TEMPLATE_IDS == null)
			{
				return null;
			}
			string[] array = new string[ITEM_TEMPLATE_IDS.Length];
			for (int i = 0; i < ITEM_TEMPLATE_IDS.Length; i++)
			{
				array[i] = getItemName(i);
			}
			return array;
		}
	}

	public static string[] PICK_NAMES => mResources.autoPick_Names;

	public static string[] BUA_NAMES => mResources.autoBua_Names;

	public Scroll scroll
	{
		get
		{
			if (scrolls != null && currentTab >= 0 && currentTab < scrolls.Length)
			{
				return scrolls[currentTab];
			}
			return null;
		}
	}

	public static string getItemName(int index)
	{
		if (index < 0 || ITEM_TEMPLATE_IDS == null || index >= ITEM_TEMPLATE_IDS.Length)
		{
			return string.Empty;
		}
		ItemTemplate template = ItemTemplates.get((short)ITEM_TEMPLATE_IDS[index]);
		return (template != null) ? template.name : string.Empty;
	}

	static AutoTrainMenu()
	{
		isChecked = new bool[3] { false, true, false };
		paintUI = new Paint();
		hpQuaiOn = 0L;
		hpQuaiDuoi = 10000000001L;
		peanHpPercent = 50;
		peanMpPercent = 50;
		listMobNames = new MyVector();
		isCheckedMobName = new bool[100];
		isCheckAllMob = true;
		mobNameSnapshotMapId = -1;
		listSkills = new MyVector();
		isCheckedSkill = new bool[50];
		isCheckItem = new bool[25];
		isCheckPick = new bool[4];
		strIdNhat = string.Empty;
		strIdLoaiTru = string.Empty;
		strIdVutBo = string.Empty;
		muaBuaKills = new int[4];
		isCheckBua = new bool[4];
		ITEM_TEMPLATE_IDS = new int[25]
		{
			521, 1628, 381, 1150, 384, 1153, 382, 1152, 383, 1151,
			1517, 1518, 663, 664, 665, 666, 667, 880, 881, 882,
			385, 1154, 211, 212, 2021
		};
		BUA_TEMPLATE_IDS = new int[4] { 213, 216, 222, 219 };
		BUA_ICON_IDS = new int[4] { 2754, 2756, 2758, 2755 };
		lastTimeUseItemByType = new long[25];
	}

	public static AutoTrainMenu gI()
	{
		if (instance == null)
		{
			instance = new AutoTrainMenu();
		}
		return instance;
	}

	public AutoTrainMenu()
	{
		w = 260;
		h = 220;
		if (w > GameCanvas.w - 20)
		{
			w = GameCanvas.w - 20;
		}
		if (h > GameCanvas.h - 30)
		{
			h = GameCanvas.h - 30;
		}
		x = (GameCanvas.w - w) / 2;
		y = (GameCanvas.h - h) / 2;
		xScroll = x + 10;
		yScroll = y + 42;
		wScroll = w - 20;
		hScroll = h - 55;

		scrolls = new Scroll[3];
		for (int i = 0; i < scrolls.Length; i++)
		{
			scrolls[i] = new Scroll();
		}

		tfHpOn = new TField();
		tfHpOn.setIputType(TField.INPUT_TYPE_NUMERIC);
		tfHpOn.width = 75;
		tfHpOn.height = 18;
		tfHpOn.setText(mResources.khongGioiHan);

		tfHpDuoi = new TField();
		tfHpDuoi.setIputType(TField.INPUT_TYPE_NUMERIC);
		tfHpDuoi.width = 75;
		tfHpDuoi.height = 18;
		tfHpDuoi.setText(mResources.khongGioiHan);

		tfHpPean = new TField();
		tfHpPean.setIputType(TField.INPUT_TYPE_NUMERIC);
		tfHpPean.width = 35;
		tfHpPean.height = 18;
		tfHpPean.setText("50");

		tfMpPean = new TField();
		tfMpPean.setIputType(TField.INPUT_TYPE_NUMERIC);
		tfMpPean.width = 35;
		tfMpPean.height = 18;
		tfMpPean.setText("50");

		tfIdNhat = new TField();
		tfIdNhat.width = 110;
		tfIdNhat.height = 18;

		tfIdLoaiTru = new TField();
		tfIdLoaiTru.width = 110;
		tfIdLoaiTru.height = 18;

		tfIdVutBo = new TField();
		tfIdVutBo.width = 110;
		tfIdVutBo.height = 18;

		tfBuaKills = new TField[4];
		for (int j = 0; j < 4; j++)
		{
			tfBuaKills[j] = new TField();
			tfBuaKills[j].setIputType(TField.INPUT_TYPE_NUMERIC);
			tfBuaKills[j].width = 40;
			tfBuaKills[j].height = 18;
			tfBuaKills[j].setText("0");
		}

		cmdClose = new Command(string.Empty, this, 1000, null);
	}

	public static void scanSkills()
	{
		if (listSkills == null)
		{
			listSkills = new MyVector();
		}
		listSkills.removeAllElements();
		Char myChar = Char.myCharz();
		if (myChar == null || myChar.vSkill == null)
		{
			return;
		}
		for (int i = 0; i < myChar.vSkill.size(); i++)
		{
			Skill skill = (Skill)myChar.vSkill.elementAt(i);
			if (skill != null && skill.point > 0 && skill.template != null && skill.template.type != 0)
			{
				listSkills.addElement(skill);
			}
		}
	}

	private static int getBuffIconId(int templateId)
	{
		switch (templateId)
		{
		case 213:
		case 381:
		case 1150:
			return 2754;
		case 216:
		case 383:
		case 1151:
			return 2756;
		case 219:
		case 382:
		case 1152:
			return 2755;
		case 222:
			return 2758;
		case 384:
		case 1153:
			return 2757;
		case 385:
		case 1154:
			return 2760;
		default:
			return -1;
		}
	}

	private static bool isItemUseOnCooldown(int itemIndex, long now)
	{
		if (itemIndex < 0 || lastTimeUseItemByType == null || itemIndex >= lastTimeUseItemByType.Length)
		{
			return false;
		}
		long num = lastTimeUseItemByType[itemIndex];
		if (num <= 0L)
		{
			return false;
		}
		return now - num < ITEM_USE_COOLDOWN_MILLIS;
	}

	private static void markItemUseSent(int itemIndex, long now)
	{
		if (itemIndex >= 0 && lastTimeUseItemByType != null && itemIndex < lastTimeUseItemByType.Length)
		{
			lastTimeUseItemByType[itemIndex] = now;
			lastTimeUseItem = now;
		}
	}

	private static int getActiveBuffKey(int index)
	{
		if (index >= 0 && BUA_ICON_IDS != null && index < BUA_ICON_IDS.Length)
		{
			return BUA_ICON_IDS[index];
		}
		return -1;
	}

	public static bool isItemBuffActive(int index)
	{
		int buffIconId = getActiveBuffKey(index);
		if (buffIconId == -1)
		{
			return false;
		}
		Char myChar = Char.myCharz();
		if (myChar == null || myChar.vBuff == null)
		{
			return false;
		}
		for (int i = 0; i < myChar.vBuff.size(); i++)
		{
			Buff buff = (Buff)myChar.vBuff.elementAt(i);
			if (buff != null && buff.iconId == buffIconId)
			{
				return true;
			}
		}
		return false;
	}

	public static void updateAutoItem()
	{
		Char myChar = Char.myCharz();
		if (myChar == null || myChar.statusMe == 14 || myChar.arrItemBag == null)
		{
			return;
		}
		long now = mSystem.currentTimeMillis();
		if (now - lastTimeUseItem < 1000L)
		{
			return;
		}
		for (int i = 0; i < ITEM_TEMPLATE_IDS.Length; i++)
		{
			if (isCheckItem == null || i >= isCheckItem.Length || !isCheckItem[i])
			{
				continue;
			}
			if (isItemUseOnCooldown(i, now))
			{
				continue;
			}
			int templateId = ITEM_TEMPLATE_IDS[i];
			for (int j = 0; j < myChar.arrItemBag.Length; j++)
			{
				Item item = myChar.arrItemBag[j];
				if (item != null && item.template != null && item.template.id == templateId)
				{
					Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
					markItemUseSent(i, now);
					return;
				}
			}
		}
	}

	private static bool isIdInList(int id, string idListStr)
	{
		if (string.IsNullOrEmpty(idListStr))
		{
			return false;
		}
		string text = idListStr.Trim();
		if (text.Length == 0)
		{
			return false;
		}
		string[] array = text.Split(new char[3] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			if (int.TryParse(array[i].Trim(), out int result) && result == id)
			{
				return true;
			}
		}
		return false;
	}

	public static void updateAutoPick()
	{
		Char myChar = Char.myCharz();
		if (myChar == null || myChar.statusMe == 14)
		{
			return;
		}
		long now = mSystem.currentTimeMillis();
		if (now - lastTimePick < 500L)
		{
			return;
		}
		if (isCheckPick != null && isCheckPick.Length > 0 && isCheckPick[0])
		{
			bool pickOthers = isCheckPick.Length > 1 && isCheckPick[1];
			bool pickAllMap = isCheckPick.Length > 2 && isCheckPick[2];
			MyVector vItemMap = GameScr.vItemMap;
			if (vItemMap != null)
			{
				for (int i = 0; i < vItemMap.size(); i++)
				{
					ItemMap itemMap = (ItemMap)vItemMap.elementAt(i);
					if (itemMap == null || itemMap.template == null)
					{
						continue;
					}
					if (!pickOthers && itemMap.playerId != myChar.charID && itemMap.playerId != -1)
					{
						continue;
					}
					if (isIdInList(itemMap.template.id, strIdLoaiTru))
					{
						continue;
					}
					if (!string.IsNullOrEmpty(strIdNhat) && !isIdInList(itemMap.template.id, strIdNhat))
					{
						continue;
					}
					int dist = Res.distance(myChar.cx, myChar.cy, itemMap.x, itemMap.y);
					if (pickAllMap || dist <= 120)
					{
						Service.gI().pickItem(itemMap.itemMapID);
						lastTimePick = now;
						return;
					}
				}
			}
		}

		if (isCheckPick != null && isCheckPick.Length > 3 && isCheckPick[3] && !string.IsNullOrEmpty(strIdVutBo))
		{
			if (now - lastTimeDrop < 1500L)
			{
				return;
			}
			if (myChar.arrItemBag != null)
			{
				for (int j = 0; j < myChar.arrItemBag.Length; j++)
				{
					Item item = myChar.arrItemBag[j];
					if (item != null && item.template != null && isIdInList(item.template.id, strIdVutBo))
					{
						Service.gI().dropItem(0, (sbyte)item.indexUI);
						lastTimeDrop = now;
						return;
					}
				}
			}
		}
	}

	public static void updateAutoBua()
	{
		Char myChar = Char.myCharz();
		if (myChar == null || myChar.statusMe == 14)
		{
			return;
		}
		long now = mSystem.currentTimeMillis();
		if (now - lastTimeBuyBua < 3000L)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (isCheckBua != null && i < isCheckBua.Length && isCheckBua[i])
			{
				if (!isItemBuffActive(i))
				{
					Service.gI().buyItem(0, BUA_TEMPLATE_IDS[i], 1);
					lastTimeBuyBua = now;
					return;
				}
			}
		}
	}

	public static void updateAutoPean()
	{
		if (!isCheckAutoPean)
		{
			return;
		}
		Char myChar = Char.myCharz();
		if (myChar == null || myChar.statusMe == 14 || GameScr.auto)
		{
			return;
		}
		long now = mSystem.currentTimeMillis();
		if (now - lastTimeUsePean < 1000L)
		{
			return;
		}
		if (myChar.cHPFull <= 0L || myChar.cMPFull <= 0L)
		{
			return;
		}
		int curHpPercent = (int)(myChar.cHP * 100L / myChar.cHPFull);
		int curMpPercent = (int)(myChar.cMP * 100L / myChar.cMPFull);
		if (curHpPercent < peanHpPercent || curMpPercent < peanMpPercent)
		{
			GameScr.gI().doUseHP();
			lastTimeUsePean = now;
		}
	}

	public static void updateAutoTrain()
	{
		updateAutoPean();
		updateAutoPick();
		updateAutoItem();
		updateAutoBua();

		if (isChecked == null || !isChecked[0])
		{
			return;
		}

		Char myChar = Char.myCharz();
		if (myChar == null || myChar.statusMe == 14 || GameScr.auto)
		{
			return;
		}

		if (isCharacterBusyAttacking(myChar))
		{
			return;
		}

		long now = mSystem.currentTimeMillis();
		Skill skill = getNextSkillToUse(now);
		if (skill == null)
		{
			return;
		}

		Mob target = findMobToAttack();
		if (target != null)
		{
			timeMapEmpty = 0L;
			myChar.mobFocus = target;
			int dist = Res.distance(myChar.cx, myChar.cy, target.x, target.y);
			int attackRange = skill.dx;
			if (attackRange <= 0)
			{
				attackRange = 40;
			}

			if (dist <= attackRange)
			{
				GameScr.gI().doUseSkillForAutoTrain(skill);
				lastTimeAttack = now;
			}
			else
			{
				myChar.currentMovePoint = new MovePoint(target.x, target.y);
			}
		}
		else if (isChecked.Length > 2 && isChecked[2])
		{
			if (timeMapEmpty == 0L)
			{
				timeMapEmpty = now;
			}
			else if (now - timeMapEmpty > 4000L && now > autoTrainNextZoneAllowedTime)
			{
				autoTrainNextZoneAllowedTime = now + 5000L;
				Service.gI().openUIZone();
			}
		}
	}

	private static bool isCharacterBusyAttacking(Char c)
	{
		if (c == null)
		{
			return false;
		}
		return c.arr != null || c.dart != null || c.effP != null || c.isTeleport || c.isPaintNewSkill || c.isAttack() || c.isCharge;
	}

	private static Skill getNextSkillToUse(long now)
	{
		if (listSkills == null || listSkills.size() == 0)
		{
			scanSkills();
		}
		if (listSkills == null)
		{
			return null;
		}
		Char myChar = Char.myCharz();
		if (myChar == null)
		{
			return null;
		}
		for (int i = 0; i < listSkills.size(); i++)
		{
			if (isCheckedSkill != null && i < isCheckedSkill.Length && isCheckedSkill[i])
			{
				Skill skill = (Skill)listSkills.elementAt(i);
				if (skill != null && isSkillReady(skill, myChar, now))
				{
					return skill;
				}
			}
		}
		return null;
	}

	private static bool isSkillReady(Skill skill, Char myChar, long now)
	{
		if (skill == null || skill.template == null || myChar == null)
		{
			return false;
		}
		long coolDown = Math.Max(500, skill.coolDown);
		if (now - skill.lastTimeUseThisSkill < coolDown)
		{
			return false;
		}
		int manaCost;
		if (skill.template.manaUseType == 2)
		{
			manaCost = 1;
		}
		else if (skill.template.manaUseType == 1)
		{
			manaCost = (int)(myChar.cMPFull * (long)skill.manaUse / 100L);
		}
		else
		{
			manaCost = skill.manaUse;
		}
		return myChar.cMP >= (long)manaCost;
	}

	private static bool isMobValid(Mob mob)
	{
		if (mob == null || mob.isMobMe || mob.hp <= 0L || mob.isBlind || mob.status == 1 || mob.status == 0)
		{
			return false;
		}
		if (mob.x == 0 && mob.y == 0)
		{
			return false;
		}
		if (isChecked != null && isChecked.Length > 1 && isChecked[1])
		{
			if (mob.isBoss() || mob.isSieuQuai)
			{
				return false;
			}
		}
		if (isCheckAttackByHp)
		{
			if (hpQuaiOn > 0L && mob.hp < hpQuaiOn)
			{
				return false;
			}
			if (hpQuaiDuoi > 0L && mob.hp > hpQuaiDuoi)
			{
				return false;
			}
		}
		if (!isCheckAllMob && listMobNames != null)
		{
			int mobTemplateId = mob.templateId;
			bool match = false;
			for (int i = 0; i < listMobNames.size(); i++)
			{
				if (isCheckedMobName != null && i < isCheckedMobName.Length && isCheckedMobName[i])
				{
					if (mob.getTemplate().name.Equals((string)listMobNames.elementAt(i)))
					{
						match = true;
						break;
					}
				}
			}
			if (!match)
			{
				return false;
			}
		}
		if (mob.mobId == unreachableMobId && mSystem.currentTimeMillis() - unreachableMobTime < 5000L)
		{
			return false;
		}
		return true;
	}

	private static Mob findMobToAttack()
	{
		Char myChar = Char.myCharz();
		if (myChar == null || GameScr.vMob == null)
		{
			return null;
		}
		int minDistance = 999999;
		Mob bestMob = null;
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = (Mob)GameScr.vMob.elementAt(i);
			if (isMobValid(mob))
			{
				int dist = Res.distance(myChar.cx, myChar.cy, mob.x, mob.y);
				if (dist < minDistance)
				{
					minDistance = dist;
					bestMob = mob;
				}
			}
		}
		return bestMob;
	}

	public static void resetMobNameSnapshot()
	{
		mobNameSnapshotReady = false;
		mobNameSnapshotMapId = -1;
	}

	public static void captureMobNameSnapshot()
	{
		if (listMobNames == null)
		{
			listMobNames = new MyVector();
		}
		listMobNames.removeAllElements();
		if (GameScr.vMob != null)
		{
			for (int i = 0; i < GameScr.vMob.size(); i++)
			{
				Mob mob = (Mob)GameScr.vMob.elementAt(i);
				if (mob != null && mob.getTemplate() != null)
				{
					string name = mob.getTemplate().name;
					if (!listMobNames.contains(name))
					{
						listMobNames.addElement(name);
					}
				}
			}
		}
		mobNameSnapshotReady = true;
		mobNameSnapshotMapId = TileMap.mapID;
	}

	private static void resetMobSelectionToAll()
	{
		isCheckAllMob = true;
		if (isCheckedMobName != null)
		{
			for (int i = 0; i < isCheckedMobName.Length; i++)
			{
				isCheckedMobName[i] = true;
			}
		}
	}

	private static void ensureMobSelectionHasAnySelected()
	{
		if (isCheckAllMob)
		{
			return;
		}
		bool hasSelected = false;
		if (listMobNames != null)
		{
			for (int i = 0; i < listMobNames.size(); i++)
			{
				if (isCheckedMobName != null && isCheckedMobName[i])
				{
					hasSelected = true;
					break;
				}
			}
		}
		if (!hasSelected)
		{
			resetMobSelectionToAll();
		}
	}

	private static string getMobSelectionRmsKey()
	{
		Char myChar = Char.myCharz();
		int charId = (myChar != null) ? myChar.charID : 0;
		return "AT_mobs_" + charId + "_" + TileMap.mapID;
	}

	private static void saveMobSelectionForCurrentMap()
	{
		try
		{
			string key = getMobSelectionRmsKey();
			string data = (isCheckAllMob ? "1" : "0");
			if (listMobNames != null)
			{
				for (int i = 0; i < listMobNames.size(); i++)
				{
					data = data + "," + (string)listMobNames.elementAt(i) + ":" + (isCheckedMobName[i] ? "1" : "0");
				}
			}
			Rms.saveRMSString(key, data);
		}
		catch (Exception)
		{
		}
	}

	private static bool loadMobSelectionForCurrentMap()
	{
		try
		{
			string key = getMobSelectionRmsKey();
			string data = Rms.loadRMSString(key);
			if (string.IsNullOrEmpty(data))
			{
				return false;
			}
			string[] parts = data.Split(',');
			if (parts.Length > 0)
			{
				isCheckAllMob = (parts[0] == "1");
				for (int i = 1; i < parts.Length; i++)
				{
					string[] pair = parts[i].Split(':');
					if (pair.Length == 2 && listMobNames != null)
					{
						int idx = listMobNames.indexOf(pair[0]);
						if (idx >= 0 && idx < isCheckedMobName.Length)
						{
							isCheckedMobName[idx] = (pair[1] == "1");
						}
					}
				}
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	private static void prepareMobSelectionForCurrentMap()
	{
		captureMobNameSnapshot();
		if (!loadMobSelectionForCurrentMap())
		{
			resetMobSelectionToAll();
		}
	}

	public static void scanMobsInMap()
	{
		prepareMobSelectionForCurrentMap();
	}

	public void clearAllFocus()
	{
		if (tfHpOn != null) tfHpOn.isFocus = false;
		if (tfHpDuoi != null) tfHpDuoi.isFocus = false;
		if (tfHpPean != null) tfHpPean.isFocus = false;
		if (tfMpPean != null) tfMpPean.isFocus = false;
		if (tfIdNhat != null) tfIdNhat.isFocus = false;
		if (tfIdLoaiTru != null) tfIdLoaiTru.isFocus = false;
		if (tfIdVutBo != null) tfIdVutBo.isFocus = false;
		if (tfBuaKills != null)
		{
			for (int i = 0; i < tfBuaKills.Length; i++)
			{
				if (tfBuaKills[i] != null) tfBuaKills[i].isFocus = false;
			}
		}
	}

	public void show()
	{
		scanSkills();
		prepareMobSelectionForCurrentMap();
		loadAutoTrainConfig();
		updateScrollHeight();
		clearAllFocus();
		switchToMe();
	}

	private void updateScrollHeight()
	{
		if (scrolls == null)
		{
			return;
		}
		if (scrolls.Length > 0 && scrolls[0] != null)
		{
			int mobCount = (listMobNames != null) ? listMobNames.size() : 0;
			int skillCount = (listSkills != null) ? listSkills.size() : 0;
			scrolls[0].nContent = 150 + mobCount * 22 + skillCount * 24;
		}
		if (scrolls.Length > 1 && scrolls[1] != null)
		{
			scrolls[1].nContent = ITEM_TEMPLATE_IDS.Length * 24 + 10;
		}
		if (scrolls.Length > 2 && scrolls[2] != null)
		{
			scrolls[2].nContent = 180;
		}
	}

	public override void paint(mGraphics g)
	{
		GameCanvas.paintz.paintFrameSimple(x, y, w, h, g);
		string[] tabNames = (mGraphics.zoomLevel > 1) ? TAB_NAMES : TAB_NAMES_2;
		if (tabNames != null)
		{
			int tabW = (w - 20) / tabNames.Length;
			for (int i = 0; i < tabNames.Length; i++)
			{
				int tabX = x + 10 + i * tabW;
				int tabY = y + 10;
				if (i == currentTab)
				{
					g.setColor(16777215);
					g.fillRect(tabX, tabY, tabW, 20);
					mFont.tahoma_7b_dark.drawString(g, tabNames[i], tabX + tabW / 2, tabY + 4, mFont.CENTER);
				}
				else
				{
					g.setColor(8421504);
					g.fillRect(tabX, tabY, tabW, 20);
					mFont.tahoma_7b_white.drawString(g, tabNames[i], tabX + tabW / 2, tabY + 4, mFont.CENTER);
				}
			}
		}

		g.setClip(xScroll, yScroll, wScroll, hScroll);
		int curY = yScroll - ((scroll != null) ? scroll.cmy : 0);

		if (currentTab == 0)
		{
			paintTabTrain(g, ref curY);
		}
		else if (currentTab == 1)
		{
			paintTabItem(g, ref curY);
		}
		else if (currentTab == 2)
		{
			paintTabPick(g, ref curY);
		}

		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	private void paintTabTrain(mGraphics g, ref int curY)
	{
		string[] options = OPTION_NAMES;
		for (int i = 0; i < options.Length; i++)
		{
			paintCheckboxItemCustom(g, options[i], xScroll + 5, curY, isChecked[i], false);
			curY += 24;
		}

		mFont.tahoma_7b_yellow.drawString(g, mResources.danhTheoHp, xScroll + 5, curY, mFont.LEFT);
		curY += 18;
		mFont.tahoma_7_white.drawString(g, mResources.hpTren, xScroll + 5, curY + 2, mFont.LEFT);
		paintTFieldCustom(g, tfHpOn, xScroll + 55, curY);
		mFont.tahoma_7_white.drawString(g, mResources.hpDuoi, xScroll + 135, curY + 2, mFont.LEFT);
		paintTFieldCustom(g, tfHpDuoi, xScroll + 185, curY);
		curY += 26;

		mFont.tahoma_7b_yellow.drawString(g, mResources.tuDungDau, xScroll + 5, curY, mFont.LEFT);
		paintCheckboxItemCustom(g, string.Empty, xScroll + 90, curY - 2, isCheckAutoPean, false);
		curY += 20;
		mFont.tahoma_7_white.drawString(g, mResources.percentHpDungDau, xScroll + 5, curY + 2, mFont.LEFT);
		paintTFieldCustom(g, tfHpPean, xScroll + 90, curY);
		mFont.tahoma_7_white.drawString(g, mResources.percentMpDungDau, xScroll + 130, curY + 2, mFont.LEFT);
		paintTFieldCustom(g, tfMpPean, xScroll + 215, curY);
		curY += 26;

		mFont.tahoma_7b_yellow.drawString(g, mResources.chonQuaiTheoTen, xScroll + 5, curY, mFont.LEFT);
		paintCheckboxItemCustom(g, mResources.tatCa, xScroll + 130, curY, isCheckAllMob, false);
		curY += 22;

		if (!isCheckAllMob && listMobNames != null)
		{
			for (int j = 0; j < listMobNames.size(); j++)
			{
				string mobName = (string)listMobNames.elementAt(j);
				paintCheckboxItemCustom(g, mobName, xScroll + 15, curY, isCheckedMobName[j], false);
				curY += 22;
			}
		}

		mFont.tahoma_7b_yellow.drawString(g, mResources.danhSachSkillTuDanh, xScroll + 5, curY, mFont.LEFT);
		curY += 22;
		paintSkillsCenteredWithLeftCB(g, curY);
		curY += 30;
	}

	private void paintTabItem(mGraphics g, ref int curY)
	{
		string[] itemNames = ITEM_NAMES;
		if (itemNames != null)
		{
			for (int i = 0; i < itemNames.Length; i++)
			{
				paintCheckboxItemCustom(g, itemNames[i], xScroll + 5, curY, isCheckItem[i], false);
				curY += 24;
			}
		}
	}

	private void paintTabPick(mGraphics g, ref int curY)
	{
		string[] pickNames = PICK_NAMES;
		if (pickNames != null)
		{
			for (int i = 0; i < pickNames.Length; i++)
			{
				paintCheckboxItemCustom(g, pickNames[i], xScroll + 5, curY, isCheckPick[i], false);
				curY += 24;
			}
		}

		mFont.tahoma_7_white.drawString(g, mResources.dsItemIdNhat, xScroll + 5, curY + 2, mFont.LEFT);
		paintTFieldCustom(g, tfIdNhat, xScroll + 105, curY);
		curY += 24;

		mFont.tahoma_7_white.drawString(g, mResources.dsItemIdLoaiTru, xScroll + 5, curY + 2, mFont.LEFT);
		paintTFieldCustom(g, tfIdLoaiTru, xScroll + 105, curY);
		curY += 24;

		mFont.tahoma_7_white.drawString(g, mResources.dsItemIdVutBo, xScroll + 5, curY + 2, mFont.LEFT);
		paintTFieldCustom(g, tfIdVutBo, xScroll + 105, curY);
		curY += 28;

		mFont.tahoma_7b_yellow.drawString(g, mResources.muaBuaHoTroKills, xScroll + 5, curY, mFont.LEFT);
		curY += 22;
		string[] buaNames = BUA_NAMES;
		if (buaNames != null)
		{
			for (int j = 0; j < buaNames.Length; j++)
			{
				paintCheckboxItemCustom(g, buaNames[j], xScroll + 5, curY, isCheckBua[j], false);
				if (tfBuaKills != null && j < tfBuaKills.Length)
				{
					paintTFieldCustom(g, tfBuaKills[j], xScroll + 115, curY);
				}
				curY += 24;
			}
		}
	}

	private void paintSkillsCenteredWithLeftCB(mGraphics g, int startY)
	{
		if (listSkills == null)
		{
			return;
		}
		for (int i = 0; i < listSkills.size(); i++)
		{
			Skill skill = (Skill)listSkills.elementAt(i);
			int itemX = xScroll + 10 + (i % 6) * 36;
			int itemY = startY + (i / 6) * 36;
			paintCheckboxItemCustom(g, string.Empty, itemX - 5, itemY, isCheckedSkill[i], false);
			if (skill != null && skill.template != null)
			{
				SmallImage.drawSmallImage(g, skill.template.iconId, itemX + 12, itemY, 0, mGraphics.TOP | mGraphics.LEFT);
			}
		}
	}

	private void paintTFieldCustom(mGraphics g, TField tf, int itemX, int itemY)
	{
		if (tf != null)
		{
			tf.x = itemX;
			tf.y = itemY;
			tf.paint(g);
		}
	}

	private void paintTFieldLine(mGraphics g, TField tf, int itemX, int itemY)
	{
		paintTFieldCustom(g, tf, itemX, itemY);
	}

	private void paintCheckboxItem(mGraphics g, int index, int itemX, int itemY, bool isCheckedItem, bool focus)
	{
		paintUI.paintCheckBox(itemX, itemY, isCheckedItem, g);
	}

	private void paintCheckboxItemCustom(mGraphics g, string name, int itemX, int itemY, bool isCheckedItem, bool focus)
	{
		paintUI.paintCheckBox(itemX, itemY, isCheckedItem, g);
		if (!string.IsNullOrEmpty(name))
		{
			mFont.tahoma_7_white.drawString(g, name, itemX + 22, itemY + 2, mFont.LEFT);
		}
	}

	public override void update()
	{
		if (scroll != null)
		{
			scroll.updateKey();
		}
		if (tfHpOn != null) tfHpOn.update();
		if (tfHpDuoi != null) tfHpDuoi.update();
		if (tfHpPean != null) tfHpPean.update();
		if (tfMpPean != null) tfMpPean.update();
		if (tfIdNhat != null) tfIdNhat.update();
		if (tfIdLoaiTru != null) tfIdLoaiTru.update();
		if (tfIdVutBo != null) tfIdVutBo.update();
		if (tfBuaKills != null)
		{
			for (int i = 0; i < tfBuaKills.Length; i++)
			{
				if (tfBuaKills[i] != null) tfBuaKills[i].update();
			}
		}
	}

	private void updateTField(TField tf, ref long value, long defaultVal)
	{
		if (tf != null)
		{
			string text = tf.getText();
			if (string.IsNullOrEmpty(text) || text == mResources.khongGioiHan)
			{
				value = defaultVal;
			}
			else if (long.TryParse(text, out long result))
			{
				value = result;
			}
		}
	}

	private void updateTFieldInt(TField tf, ref int value)
	{
		if (tf != null && int.TryParse(tf.getText(), out int result))
		{
			value = result;
		}
	}

	public static void resetToDefault()
	{
		isChecked = new bool[3] { false, true, false };
		hpQuaiOn = 0L;
		hpQuaiDuoi = 10000000001L;
		peanHpPercent = 50;
		peanMpPercent = 50;
		isCheckAutoPean = false;
		isCheckAttackByHp = false;
		resetMobSelectionToAll();
		if (isCheckedSkill != null)
		{
			for (int i = 0; i < isCheckedSkill.Length; i++)
			{
				isCheckedSkill[i] = (i == 0);
			}
		}
		if (isCheckItem != null)
		{
			for (int j = 0; j < isCheckItem.Length; j++)
			{
				isCheckItem[j] = false;
			}
		}
		if (isCheckPick != null)
		{
			for (int k = 0; k < isCheckPick.Length; k++)
			{
				isCheckPick[k] = false;
			}
		}
		strIdNhat = string.Empty;
		strIdLoaiTru = string.Empty;
		strIdVutBo = string.Empty;
	}

	public static void saveAutoTrainConfig()
	{
		try
		{
			Char myChar = Char.myCharz();
			int charId = (myChar != null) ? myChar.charID : 0;
			string key = "AT_v6_" + charId;
			string data = (isChecked[0] ? "1" : "0") + "," + (isChecked[1] ? "1" : "0") + "," + (isChecked[2] ? "1" : "0")
				+ "," + hpQuaiOn + "," + hpQuaiDuoi
				+ "," + (isCheckAutoPean ? "1" : "0") + "," + peanHpPercent + "," + peanMpPercent
				+ "," + strIdNhat + "," + strIdLoaiTru + "," + strIdVutBo;
			Rms.saveRMSString(key, data);
			saveMobSelectionForCurrentMap();
		}
		catch (Exception)
		{
		}
	}

	public static void loadAutoTrainConfig()
	{
		try
		{
			Char myChar = Char.myCharz();
			int charId = (myChar != null) ? myChar.charID : 0;
			string key = "AT_v6_" + charId;
			string data = Rms.loadRMSString(key);
			if (string.IsNullOrEmpty(data))
			{
				key = "AT_v5_" + charId;
				data = Rms.loadRMSString(key);
			}
			if (!string.IsNullOrEmpty(data))
			{
				string[] parts = data.Split(',');
				if (parts.Length >= 11)
				{
					isChecked[0] = (parts[0] == "1");
					isChecked[1] = (parts[1] == "1");
					isChecked[2] = (parts[2] == "1");
					long.TryParse(parts[3], out hpQuaiOn);
					long.TryParse(parts[4], out hpQuaiDuoi);
					isCheckAutoPean = (parts[5] == "1");
					int.TryParse(parts[6], out peanHpPercent);
					int.TryParse(parts[7], out peanMpPercent);
					strIdNhat = parts[8];
					strIdLoaiTru = parts[9];
					strIdVutBo = parts[10];
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public override void updateKey()
	{
		if (GameCanvas.isPointerClick)
		{
			int clickX = GameCanvas.px;
			int clickY = GameCanvas.py;
			if (clickX < x || clickX > x + w || clickY < y || clickY > y + h)
			{
				saveAutoTrainConfig();
				GameScr.gI().switchToMe();
				return;
			}

			string[] tabNames = (mGraphics.zoomLevel > 1) ? TAB_NAMES : TAB_NAMES_2;
			if (tabNames != null)
			{
				int tabW = (w - 20) / tabNames.Length;
				for (int i = 0; i < tabNames.Length; i++)
				{
					int tabX = x + 10 + i * tabW;
					int tabY = y + 10;
					if (clickX >= tabX && clickX <= tabX + tabW && clickY >= tabY && clickY <= tabY + 20)
					{
						currentTab = i;
						clearAllFocus();
						updateScrollHeight();
						return;
					}
				}
			}

			if (clickX >= xScroll && clickX <= xScroll + wScroll && clickY >= yScroll && clickY <= yScroll + hScroll)
			{
				if (currentTab == 0) updateKeyTabTrain(clickY);
				else if (currentTab == 1) updateKeyTabItem(clickY);
				else if (currentTab == 2) updateKeyTabPick(clickY);
			}
		}
		base.updateKey();
	}

	private void clearTouchState()
	{
	}

	private void resetScrollTouchState()
	{
	}

	private bool checkTouchTField(TField tf, int screenY)
	{
		if (tf != null)
		{
			return GameCanvas.px >= tf.x && GameCanvas.px <= tf.x + tf.width && screenY >= tf.y && screenY <= tf.y + tf.height;
		}
		return false;
	}

	public static bool isTouchCheckbox(int xCB, int yCB)
	{
		return GameCanvas.px >= xCB && GameCanvas.px <= xCB + 18 && GameCanvas.py >= yCB && GameCanvas.py <= yCB + 18;
	}

	public override void keyPress(int keyCode)
	{
		if (tfHpOn != null && tfHpOn.isFocus) handleHpKeyPress(tfHpOn, keyCode);
		else if (tfHpDuoi != null && tfHpDuoi.isFocus) handleHpKeyPress(tfHpDuoi, keyCode);
		else if (tfHpPean != null && tfHpPean.isFocus) handlePercentKeyPress(tfHpPean, keyCode);
		else if (tfMpPean != null && tfMpPean.isFocus) handlePercentKeyPress(tfMpPean, keyCode);
		base.keyPress(keyCode);
	}

	private void handlePercentKeyPress(TField tf, int keyCode)
	{
		tf.keyPressed(keyCode);
		if (int.TryParse(tf.getText(), out int val))
		{
			if (val > 100) tf.setText("100");
			else if (val < 0) tf.setText("0");
		}
	}

	private void handleHpKeyPress(TField tf, int keyCode)
	{
		tf.keyPressed(keyCode);
	}

	private static bool isTextContainsLetters(string str)
	{
		if (string.IsNullOrEmpty(str)) return false;
		for (int i = 0; i < str.Length; i++)
		{
			if (char.IsLetter(str[i])) return true;
		}
		return false;
	}

	private void focusTField(TField tf)
	{
		clearAllFocus();
		if (tf != null) tf.isFocus = true;
	}

	private void updateFocusState()
	{
	}

	public static void setAutoAttack(bool enable, string reason)
	{
		if (isChecked != null && isChecked.Length > 0)
		{
			isChecked[0] = enable;
			if (enable)
			{
				ensureSkillSelectionForAutoAttack();
			}
		}
	}

	private static void ensureSkillSelectionForAutoAttack()
	{
		if (listSkills == null || listSkills.size() == 0)
		{
			scanSkills();
		}
		bool hasSkill = false;
		if (isCheckedSkill != null)
		{
			for (int i = 0; i < isCheckedSkill.Length; i++)
			{
				if (isCheckedSkill[i])
				{
					hasSkill = true;
					break;
				}
			}
			if (!hasSkill && isCheckedSkill.Length > 0)
			{
				isCheckedSkill[0] = true;
			}
		}
	}

	private void toggleOption(int index)
	{
		if (isChecked != null && index >= 0 && index < isChecked.Length)
		{
			isChecked[index] = !isChecked[index];
		}
	}

	public void perform(int idAction, object p)
	{
		if (idAction == 1000)
		{
			saveAutoTrainConfig();
			GameScr.gI().switchToMe();
		}
	}

	private void updateKeyTabTrain(int screenY)
	{
		int curY = yScroll - ((scroll != null) ? scroll.cmy : 0);
		for (int i = 0; i < 3; i++)
		{
			if (screenY >= curY && screenY <= curY + 22)
			{
				toggleOption(i);
				return;
			}
			curY += 24;
		}
		curY += 18; // HP label
		if (checkTouchTField(tfHpOn, screenY)) { focusTField(tfHpOn); return; }
		if (checkTouchTField(tfHpDuoi, screenY)) { focusTField(tfHpDuoi); return; }
		curY += 26;

		if (screenY >= curY && screenY <= curY + 20)
		{
			isCheckAutoPean = !isCheckAutoPean;
			return;
		}
		curY += 20;
		if (checkTouchTField(tfHpPean, screenY)) { focusTField(tfHpPean); return; }
		if (checkTouchTField(tfMpPean, screenY)) { focusTField(tfMpPean); return; }
		curY += 26;

		if (screenY >= curY && screenY <= curY + 20)
		{
			isCheckAllMob = !isCheckAllMob;
			return;
		}
		curY += 22;

		if (!isCheckAllMob && listMobNames != null)
		{
			for (int j = 0; j < listMobNames.size(); j++)
			{
				if (screenY >= curY && screenY <= curY + 20)
				{
					isCheckedMobName[j] = !isCheckedMobName[j];
					ensureMobSelectionHasAnySelected();
					return;
				}
				curY += 22;
			}
		}

		curY += 22; // skill label
		if (listSkills != null)
		{
			for (int k = 0; k < listSkills.size(); k++)
			{
				int itemX = xScroll + 10 + (k % 6) * 36;
				int itemY = curY + (k / 6) * 36;
				if (GameCanvas.px >= itemX - 5 && GameCanvas.px <= itemX + 32 && screenY >= itemY && screenY <= itemY + 32)
				{
					isCheckedSkill[k] = !isCheckedSkill[k];
					return;
				}
			}
		}
	}

	private void updateKeyTabItem(int screenY)
	{
		int curY = yScroll - ((scroll != null) ? scroll.cmy : 0);
		for (int i = 0; i < ITEM_TEMPLATE_IDS.Length; i++)
		{
			if (screenY >= curY && screenY <= curY + 22)
			{
				isCheckItem[i] = !isCheckItem[i];
				return;
			}
			curY += 24;
		}
	}

	private void updateKeyTabPick(int screenY)
	{
		int curY = yScroll - ((scroll != null) ? scroll.cmy : 0);
		for (int i = 0; i < 4; i++)
		{
			if (screenY >= curY && screenY <= curY + 22)
			{
				isCheckPick[i] = !isCheckPick[i];
				return;
			}
			curY += 24;
		}

		if (checkTouchTField(tfIdNhat, screenY)) { focusTField(tfIdNhat); return; }
		curY += 24;
		if (checkTouchTField(tfIdLoaiTru, screenY)) { focusTField(tfIdLoaiTru); return; }
		curY += 24;
		if (checkTouchTField(tfIdVutBo, screenY)) { focusTField(tfIdVutBo); return; }
		curY += 28;

		curY += 22; // Bua label
		for (int j = 0; j < 4; j++)
		{
			if (tfBuaKills != null && j < tfBuaKills.Length && checkTouchTField(tfBuaKills[j], screenY))
			{
				focusTField(tfBuaKills[j]);
				return;
			}
			if (screenY >= curY && screenY <= curY + 22)
			{
				isCheckBua[j] = !isCheckBua[j];
				return;
			}
			curY += 24;
		}
	}

	public void Dispose()
	{
	}
}
