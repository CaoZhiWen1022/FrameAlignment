import initSqlJs, { Database as SqlJsDatabase } from 'sql.js';
import * as fs from 'fs';
import * as path from 'path';

/**
 * 用户数据结构
 */
export interface User {
  id: number;
  account: string;
  password: string;
  nickname: string;
  create_time: number;
  last_login_time: number;
  use_hero_id: number;
}

/**
 * SQLite 数据库服务
 */
export class Database {
  private static instance: Database;
  private db: SqlJsDatabase | null = null;
  private dbPath: string;
  private initialized = false;

  private constructor() {
    this.dbPath = path.join(__dirname, '../../data/database.sqlite');
  }

  /**
   * 获取数据库单例
   */
  static getInstance(): Database {
    if (!Database.instance) {
      Database.instance = new Database();
    }
    return Database.instance;
  }

  /**
   * 初始化数据库
   */
  async initialize(): Promise<void> {
    if (this.initialized) return;

    const dataDir = path.dirname(this.dbPath);
    if (!fs.existsSync(dataDir)) {
      fs.mkdirSync(dataDir, { recursive: true });
    }

    const SQL = await initSqlJs();

    // 如果数据库文件存在，加载它
    if (fs.existsSync(this.dbPath)) {
      const fileBuffer = fs.readFileSync(this.dbPath);
      this.db = new SQL.Database(fileBuffer);
    } else {
      this.db = new SQL.Database();
    }

    // 创建用户表
    this.db.run(`
      CREATE TABLE IF NOT EXISTS users (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        account TEXT UNIQUE NOT NULL,
        password TEXT NOT NULL,
        nickname TEXT NOT NULL,
        create_time INTEGER NOT NULL,
        last_login_time INTEGER NOT NULL,
        use_hero_id INTEGER NOT NULL DEFAULT 1001
      )
    `);

    // 添加 use_hero_id 列（如果不存在）
    try {
      this.db.run(`ALTER TABLE users ADD COLUMN use_hero_id INTEGER NOT NULL DEFAULT 1001`);
    } catch {
      // 列已存在，忽略错误
    }

    // 迁移：将现有用户的 use_hero_id 从 0 更新为 1001
    this.db.run(`UPDATE users SET use_hero_id = 1001 WHERE use_hero_id = 0`);

    this.save();
    this.initialized = true;
    console.log('✓ SQLite 数据库初始化完成');
  }

  /**
   * 保存数据库到文件
   */
  private save(): void {
    if (!this.db) return;
    const data = this.db.export();
    const buffer = Buffer.from(data);
    fs.writeFileSync(this.dbPath, buffer);
  }

  /**
   * 根据账号查找用户
   */
  findUserByAccount(account: string): User | null {
    if (!this.db) return null;
    const result = this.db.exec(`SELECT * FROM users WHERE account = ?`, [account]);
    if (result.length === 0 || result[0].values.length === 0) return null;
    return this.rowToUser(result[0].values[0]);
  }

  /**
   * 根据 ID 查找用户
   */
  findUserById(id: number): User | null {
    if (!this.db) return null;
    const result = this.db.exec(`SELECT * FROM users WHERE id = ?`, [id]);
    if (result.length === 0 || result[0].values.length === 0) return null;
    return this.rowToUser(result[0].values[0]);
  }

  /**
   * 创建新用户
   */
  createUser(account: string, password: string, nickname: string): User {
    if (!this.db) throw new Error('数据库未初始化');
    
    const now = Date.now();
    this.db.run(
      `INSERT INTO users (account, password, nickname, create_time, last_login_time) VALUES (?, ?, ?, ?, ?)`,
      [account, password, nickname, now, now]
    );
    this.save();

    // 获取新创建的用户
    const result = this.db.exec(`SELECT last_insert_rowid()`);
    const id = result[0].values[0][0] as number;

    return {
      id,
      account,
      password,
      nickname,
      create_time: now,
      last_login_time: now,
      use_hero_id: 1001,
    };
  }

  /**
   * 更新最后登录时间
   */
  updateLastLoginTime(userId: number): void {
    if (!this.db) return;
    const now = Date.now();
    this.db.run(`UPDATE users SET last_login_time = ? WHERE id = ?`, [now, userId]);
    this.save();
  }

  /**
   * 获取用户当前使用的英雄 ID
   */
  getUseHeroId(userId: number): number {
    const user = this.findUserById(userId);
    return user?.use_hero_id ?? 1001;
  }

  /**
   * 设置用户当前使用的英雄 ID
   */
  setUseHeroId(userId: number, heroId: number): void {
    if (!this.db) return;
    this.db.run(`UPDATE users SET use_hero_id = ? WHERE id = ?`, [heroId, userId]);
    this.save();
  }

  /**
   * 将数据库行转换为 User 对象
   */
  private rowToUser(row: any[]): User {
    return {
      id: row[0] as number,
      account: row[1] as string,
      password: row[2] as string,
      nickname: row[3] as string,
      create_time: row[4] as number,
      last_login_time: row[5] as number,
      use_hero_id: (row[6] as number) ?? 1001,
    };
  }
}

export const db = Database.getInstance();

